namespace TaskTrack.Application.DTOs;

public sealed record CreatePlanejamentoRequest(
    Guid SolicitacaoId,
    DateTimeOffset? DataInicioPrevista,
    DateTimeOffset? DataFimPrevista,
    string? Observacoes,
    IReadOnlyCollection<Guid>? ResponsavelIds,
    IReadOnlyCollection<PlanejamentoMaterialRequest>? Materiais);
