using TaskTrack.Application.DTOs;
using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.Interfaces;

public interface ISolicitacoesService
{
    Task<SolicitacaoResponse> CreateAsync(CreateSolicitacaoRequest request, CancellationToken cancellationToken = default);
    Task<SolicitacaoResponse> UpdateAsync(Guid id, UpdateSolicitacaoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SolicitacaoResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SolicitacaoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetPendentesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByStatusAsync(SolicitacaoStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByGestorIdAsync(Guid gestorId, CancellationToken cancellationToken = default);
}
