using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Interfaces;
using TaskTrack.Infrastructure.Persistence;

namespace TaskTrack.Infrastructure.Repositories;

public sealed class PlanejamentosRepository : IPlanejamentosRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PlanejamentosRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<Planejamento>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _dbContext.Planejamentos
            .AsNoTracking()
            .Include(x => x.Responsaveis)
            .Include(x => x.Materiais)
            .OrderByDescending(x => x.DataInicioPrevista)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);

        return items;
    }

    public Task<Planejamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Planejamentos
            .AsNoTracking()
            .Include(x => x.Responsaveis)
            .Include(x => x.Materiais)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<Planejamento?> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Planejamentos
            .AsNoTracking()
            .Include(x => x.Responsaveis)
            .Include(x => x.Materiais)
            .FirstOrDefaultAsync(x => x.SolicitacaoId == solicitacaoId, cancellationToken);
    }

    public Task<Planejamento?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Planejamentos
            .Include(x => x.Responsaveis)
            .Include(x => x.Materiais)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> SolicitacaoExistsAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Solicitacoes
            .AsNoTracking()
            .AnyAsync(x => x.Id == solicitacaoId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Usuario>> GetUsuariosByIdsAsync(IReadOnlyCollection<Guid> usuarioIds, CancellationToken cancellationToken = default)
    {
        if (usuarioIds.Count == 0)
        {
            return [];
        }

        var usuarios = await _dbContext.Users
            .AsNoTracking()
            .Where(x => usuarioIds.Contains(x.Id))
            .Select(x => new Usuario
            {
                Id = x.Id,
                UserName = x.UserName,
                Email = x.Email
            })
            .ToListAsync(cancellationToken);

        return usuarios;
    }

    public Task AddAsync(Planejamento planejamento, CancellationToken cancellationToken = default)
    {
        return _dbContext.Planejamentos.AddAsync(planejamento, cancellationToken).AsTask();
    }

    public void Remove(Planejamento planejamento)
    {
        _dbContext.Planejamentos.Remove(planejamento);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
