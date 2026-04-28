using TaskTrack.Domain.Entities;

namespace TaskTrack.Domain.Interfaces;

public interface IExecucoesRepository
{
    Task AddAsync(Execucao execucao, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Execucao>> GetHistoricoBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<Execucao?> GetLatestBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}