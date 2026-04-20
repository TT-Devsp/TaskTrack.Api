using TaskTrack.Domain.Entities;

namespace TaskTrack.Domain.Interfaces;

public interface ISolicitacoesRepository
{
    Task AddAsync(Solicitacao solicitacao, CancellationToken cancellationToken = default);
    Task<bool> SolicitanteExistsAsync(Guid solicitanteId, CancellationToken cancellationToken = default);
    Task<bool> HasGestorApprovalAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<Solicitacao?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Solicitacao?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken = default);
    void Remove(Solicitacao solicitacao);
    Task<IReadOnlyCollection<Solicitacao>> GetAllAsync(CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
