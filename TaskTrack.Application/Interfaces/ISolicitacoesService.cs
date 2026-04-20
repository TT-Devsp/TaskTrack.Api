using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface ISolicitacoesService
{
    Task<SolicitacaoResponse> CreateAsync(CreateSolicitacaoRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SolicitacaoResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SolicitacaoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
