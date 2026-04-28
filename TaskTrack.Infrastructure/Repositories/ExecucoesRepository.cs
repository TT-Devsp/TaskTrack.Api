using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Interfaces;
using TaskTrack.Infrastructure.Persistence;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class ExecucoesRepository : IExecucoesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ExecucoesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Execucao execucao, CancellationToken cancellationToken = default)
    {
        return _dbContext.Execucoes.AddAsync(execucao, cancellationToken).AsTask();
    }

    public async Task<IReadOnlyCollection<Execucao>> GetHistoricoBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Execucoes
            .AsNoTracking()
            .Where(x => x.SolicitacaoId == solicitacaoId)
            .OrderByDescending(x => x.AtualizadoEm)
            .ToListAsync(cancellationToken);

        return items;
    }

    public Task<Execucao?> GetLatestBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Execucoes
            .AsNoTracking()
            .Where(x => x.SolicitacaoId == solicitacaoId)
            .OrderByDescending(x => x.AtualizadoEm)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}