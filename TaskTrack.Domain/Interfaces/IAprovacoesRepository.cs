using TaskTrack.Domain.Entities;

namespace TaskTrack.Domain.Interfaces;

public interface IAprovacoesRepository
{
    Task AddAsync(AprovacaoSolicitacao aprovacao, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AprovacaoSolicitacao>> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
