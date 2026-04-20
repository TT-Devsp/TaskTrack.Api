using TaskTrack.Domain.Entities;

namespace TaskTrack.Domain.Interfaces;

public interface ISolicitacoesRepository
{
    Task AddAsync(Solicitacao solicitacao, CancellationToken cancellationToken = default);
    Task<bool> SolicitanteExistsAsync(Guid solicitanteId, CancellationToken cancellationToken = default);
    Task<Solicitacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Solicitacao>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
