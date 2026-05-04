using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetRolesAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> IsInRoleAsync(User user, string role, CancellationToken cancellationToken = default);
    Task AddToRoleAsync(User user, string role, CancellationToken cancellationToken = default);
    Task RemoveFromRolesAsync(User user, IEnumerable<string> roles, CancellationToken cancellationToken = default);
    Task<bool> RoleExistsAsync(string role, CancellationToken cancellationToken = default);
    Task CreateRoleAsync(string role, CancellationToken cancellationToken = default);
    Task<bool> CheckPasswordAsync(User user, string password, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, string password, CancellationToken cancellationToken = default);
    Task DeleteAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
