using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IAprovacoesService
{
    Task<AprovacaoResponse> CreateAsync(CreateAprovacaoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AprovacaoResponse>> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
}
