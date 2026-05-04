using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Constants;

namespace TaskTrack.Application.Services;

public sealed class AdminService : IAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public AdminService(IUserRepository userRepository, IUserProfileRepository userProfileRepository)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<IReadOnlyCollection<UserWithRoleDto>> GetAllUsersAsync(string? roleFilter = null, CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var result = new List<UserWithRoleDto>();

        var normalizedFilter = string.IsNullOrWhiteSpace(roleFilter)
            ? null
            : roleFilter.Trim();

        foreach (var user in users)
        {
            var roles = await _userRepository.GetRolesAsync(user, cancellationToken);
            var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
            if (normalizedFilter is not null)
            {
                if (!roles.Contains(normalizedFilter, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }
            }

            result.Add(new UserWithRoleDto(user.Id, user.UserName, user.Email, profile?.FullName, roles));
        }

        return result;
    }

    public async Task<UserWithRoleDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var roles = await _userRepository.GetRolesAsync(user, cancellationToken);
        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        return new UserWithRoleDto(user.Id, user.UserName, user.Email, profile?.FullName, roles);
    }

    public async Task<UserWithRoleDto> UpdateUserRoleAsync(Guid userId, string newRole, CancellationToken cancellationToken = default)
    {
        if (!Roles.All.Contains(newRole))
        {
            throw new ArgumentException($"Role inválida. Roles válidas: {string.Join(", ", Roles.All)}");
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado");
        }

        if (!await _userRepository.RoleExistsAsync(newRole, cancellationToken))
        {
            await _userRepository.CreateRoleAsync(newRole, cancellationToken);
        }

        var currentRoles = await _userRepository.GetRolesAsync(user, cancellationToken);
        if (currentRoles.Any())
        {
            await _userRepository.RemoveFromRolesAsync(user, currentRoles, cancellationToken);
        }

        await _userRepository.AddToRoleAsync(user, newRole, cancellationToken);

        var updatedRoles = await _userRepository.GetRolesAsync(user, cancellationToken);
        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        return new UserWithRoleDto(user.Id, user.UserName, user.Email, profile?.FullName, updatedRoles);
    }

    public async Task DeleteUserAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default)
    {
        if (currentUserId == id)
        {
            throw new ArgumentException("Você não pode excluir seu próprio usuário");
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException("Usuário não encontrado");
        }

        await _userRepository.DeleteAsync(user, cancellationToken);
    }
}
