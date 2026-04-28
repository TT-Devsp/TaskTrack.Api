using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Interfaces;
using TaskTrack.Infrastructure.Persistence;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class AprovacoesRepository : IAprovacoesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AprovacoesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(AprovacaoSolicitacao aprovacao, CancellationToken cancellationToken = default)
    {
        return _dbContext.AprovacoesSolicitacao.AddAsync(aprovacao, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<AprovacaoSolicitacao>> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.AprovacoesSolicitacao
            .AsNoTracking()
            .Where(x => x.SolicitacaoId == solicitacaoId)
            .OrderByDescending(x => x.DataAprovacao)
            .ToListAsync(cancellationToken);

        return items;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
