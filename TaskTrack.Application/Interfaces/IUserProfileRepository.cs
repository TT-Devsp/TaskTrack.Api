using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Interfaces;

public interface IUserProfileRepository
{
    Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpsertAsync(UserProfile profile, CancellationToken cancellationToken = default);
}
