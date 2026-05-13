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

    public async Task AddAsync(Solicitacao solicitacao, CancellationToken cancellationToken = default)
    {
        await _dbContext.Solicitacoes.AddAsync(solicitacao, cancellationToken);
    }

    public Task<bool> SolicitanteExistsAsync(Guid solicitanteId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AsNoTracking().AnyAsync(x => x.Id == solicitanteId, cancellationToken);
    }

    public Task<bool> HasGestorApprovalAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.AprovacoesSolicitacao
            .AsNoTracking()
            .AnyAsync(x => x.SolicitacaoId == solicitacaoId && x.Aprovado, cancellationToken);
    }

    public async Task<Solicitacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Solicitacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Solicitacao?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Solicitacoes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Remove(Solicitacao solicitacao) => _dbContext.Solicitacoes.Remove(solicitacao);

    public async Task<IReadOnlyCollection<Solicitacao>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Solicitacoes
            .AsNoTracking()
            .OrderByDescending(x => x.DataCriacao)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetPendentesAsync(CancellationToken cancellationToken = default)
    {
        // MELHORIA: Filtro focado apenas no status Pendente para garantir que apareça na tela de aprovações
        return await _dbContext.Solicitacoes
            .Where(x => x.Status == SolicitacaoStatus.Pendente)
            .OrderByDescending(x => x.DataCriacao)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetByStatusAsync(SolicitacaoStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Solicitacoes
            .Where(x => x.Status == status)
            .OrderByDescending(x => x.DataCriacao)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Solicitacao>> GetByGestorIdAsync(Guid gestorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Solicitacoes
            .Where(x => x.GestorResponsavelId == gestorId)
            .OrderByDescending(x => x.DataCriacao)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) 
        => _dbContext.SaveChangesAsync(cancellationToken);
}