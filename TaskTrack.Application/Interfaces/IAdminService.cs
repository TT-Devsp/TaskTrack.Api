using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IAdminService
{
    Task<IReadOnlyCollection<UserWithRoleDto>> GetAllUsersAsync(string? roleFilter = null, CancellationToken cancellationToken = default);
    Task<UserWithRoleDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserWithRoleDto> UpdateUserRoleAsync(Guid userId, string newRole, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(Guid id, Guid currentUserId, CancellationToken cancellationToken = default);
}

public record UserWithRoleDto(Guid Id, string? UserName, string? Email, string? FullName, IReadOnlyList<string> Roles);
