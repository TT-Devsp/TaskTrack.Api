using TaskTrack.Domain.Entities;

namespace TaskTrack.Domain.Interfaces;

public interface IPlanejamentosRepository
{
    Task<IReadOnlyCollection<Planejamento>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Planejamento?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Planejamento?> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<Planejamento?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> SolicitacaoExistsAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Usuario>> GetUsuariosByIdsAsync(IReadOnlyCollection<Guid> usuarioIds, CancellationToken cancellationToken = default);
    Task AddAsync(Planejamento planejamento, CancellationToken cancellationToken = default);
    void Remove(Planejamento planejamento);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
