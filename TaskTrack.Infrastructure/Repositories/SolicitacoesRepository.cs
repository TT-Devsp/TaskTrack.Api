using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Domain.Interfaces;
using TaskTrack.Infrastructure.Persistence;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class SolicitacoesRepository : ISolicitacoesRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SolicitacoesRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(Solicitacao solicitacao, CancellationToken cancellationToken = default)
    {
        return _dbContext.Solicitacoes.AddAsync(solicitacao, cancellationToken).AsTask();
    }

    public Task<bool> SolicitanteExistsAsync(Guid solicitanteId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .AsNoTracking()
            .AnyAsync(x => x.Id == solicitanteId, cancellationToken);
    }

    public Task<bool> HasGestorApprovalAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.AprovacoesSolicitacao
            .AsNoTracking()
            .AnyAsync(x => x.SolicitacaoId == solicitacaoId && x.Aprovado, cancellationToken);
    }

    public Task<Solicitacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Solicitacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Solicitacao?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Solicitacoes
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Remove(Solicitacao solicitacao)
    {
        _dbContext.Solicitacoes.Remove(solicitacao);
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Solicitacoes
            .AsNoTracking()
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetPendentesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Solicitacoes
            .AsNoTracking()
            .Where(x => x.GestorResponsavelId == null
                        && x.Status == SolicitacaoStatus.Pendente
                        && !_dbContext.Planejamentos.Any(p => p.SolicitacaoId == x.Id))
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetByStatusAsync(SolicitacaoStatus status, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Solicitacoes
            .AsNoTracking()
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

        return items;
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetByGestorIdAsync(Guid gestorId, CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Solicitacoes
            .AsNoTracking()
            .Where(x => x.GestorResponsavelId == gestorId)
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);

        return items;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
