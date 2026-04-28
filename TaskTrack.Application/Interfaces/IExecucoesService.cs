using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IExecucoesService
{
    Task<ExecucaoResponse> StartAsync(StartExecucaoRequest request, CancellationToken cancellationToken = default);
    Task<ExecucaoResponse> UpdateStatusAsync(Guid solicitacaoId, UpdateExecucaoStatusRequest request, CancellationToken cancellationToken = default);
    Task<ExecucaoResponse?> GetLatestBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ExecucaoResponse>> GetHistoricoBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
}