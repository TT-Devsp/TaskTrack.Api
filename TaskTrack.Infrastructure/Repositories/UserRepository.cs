using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Application.Interfaces;
using TaskTrack.Infrastructure.Identity;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public UserRepository(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(id.ToString());
        return appUser == null ? null : MapToUser(appUser);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByEmailAsync(email);
        return appUser == null ? null : MapToUser(appUser);
    }

    public async Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var appUsers = await _userManager.Users.ToListAsync(cancellationToken);
        return appUsers.Select(MapToUser).ToList();
    }

    public async Task<IReadOnlyList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) return new List<string>();
        var roles = await _userManager.GetRolesAsync(appUser);
        return roles.ToList();
    }

    public async Task<bool> IsInRoleAsync(User user, string role, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) return false;
        return await _userManager.IsInRoleAsync(appUser, role);
    }

    public async Task AddToRoleAsync(User user, string role, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) throw new KeyNotFoundException("Usuário não encontrado");
        var result = await _userManager.AddToRoleAsync(appUser, role);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Falha ao adicionar role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task RemoveFromRolesAsync(User user, IEnumerable<string> roles, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) throw new KeyNotFoundException("Usuário não encontrado");
        var result = await _userManager.RemoveFromRolesAsync(appUser, roles);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Falha ao remover roles: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default)
    {
        return await _roleManager.RoleExistsAsync(role);
    }

    public async Task CreateRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        var result = await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Falha ao criar role: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) return false;
        return await _userManager.CheckPasswordAsync(appUser, password);
    }

    public async Task<User> CreateAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        var appUser = new ApplicationUser
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            NormalizedEmail = user.NormalizedEmail,
            NormalizedUserName = user.NormalizedUserName
        };
        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            throw new ArgumentException($"Falha ao criar usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        return MapToUser(appUser);
    }

    public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
    {
        var appUser = await _userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) throw new KeyNotFoundException("Usuário não encontrado");
        var result = await _userManager.DeleteAsync(appUser);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Falha ao excluir usuário: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    private static User MapToUser(ApplicationUser appUser)
    {
        return new User
        {
            Id = appUser.Id,
            UserName = appUser.UserName,
            Email = appUser.Email,
            NormalizedEmail = appUser.NormalizedEmail,
            NormalizedUserName = appUser.NormalizedUserName
        };
    }
}
