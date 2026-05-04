using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Constants;
using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly IUserProfileRepository _userProfileRepository;

    public AuthService(IUserRepository userRepository, IJwtService jwtService, IUserProfileRepository userProfileRepository)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            return null;
        }

        var isPasswordValid = await _userRepository.CheckPasswordAsync(user, request.Password, cancellationToken);
        if (!isPasswordValid)
        {
            return null;
        }

        var roles = await _userRepository.GetRolesAsync(user, cancellationToken);
        var role = roles.Contains(Roles.Admin)
            ? Roles.Admin
            : roles.FirstOrDefault() ?? Roles.Visualizador;

        var profile = await _userProfileRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var nome = profile?.FullName ?? user.UserName ?? user.Email ?? string.Empty;

        return _jwtService.GenerateToken(user.Id, user.Email!, nome, role);
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
        {
            throw new ArgumentException("Email já está em uso");
        }

        const string defaultRole = Roles.Visualizador;

        if (!await _userRepository.RoleExistsAsync(defaultRole, cancellationToken))
        {
            await _userRepository.CreateRoleAsync(defaultRole, cancellationToken);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Email,
            Email = request.Email,
            NormalizedEmail = request.Email.ToUpperInvariant(),
            NormalizedUserName = request.Email.ToUpperInvariant()
        };

        var createdUser = await _userRepository.CreateAsync(user, request.Password, cancellationToken);
        await _userRepository.AddToRoleAsync(createdUser, defaultRole, cancellationToken);

        await _userProfileRepository.UpsertAsync(new UserProfile
        {
            UserId = createdUser.Id,
            FullName = request.Nome
        }, cancellationToken);

        return _jwtService.GenerateToken(createdUser.Id, createdUser.Email, request.Nome, defaultRole);
    }
}
