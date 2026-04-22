using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IPlanejamentosService
{
    Task<IReadOnlyCollection<PlanejamentoResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PlanejamentoResponse?> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default);
    Task<PlanejamentoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PlanejamentoResponse> CreateAsync(CreatePlanejamentoRequest request, CancellationToken cancellationToken = default);
    Task<PlanejamentoResponse> UpdateAsync(Guid id, UpdatePlanejamentoRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}