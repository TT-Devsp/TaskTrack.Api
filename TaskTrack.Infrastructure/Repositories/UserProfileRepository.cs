using Microsoft.EntityFrameworkCore;
using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Entities;
using TaskTrack.Infrastructure.Persistence;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class UserProfileRepository : IUserProfileRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserProfileRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<UserProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task UpsertAsync(UserProfile profile, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.UserProfiles
            .FirstOrDefaultAsync(x => x.UserId == profile.UserId, cancellationToken);

        if (existing == null)
        {
            _dbContext.UserProfiles.Add(profile);
        }
        else
        {
            existing.FullName = profile.FullName;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
