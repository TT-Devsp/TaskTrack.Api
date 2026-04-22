namespace TaskTrack.Application.DTOs;

public sealed record UpdatePlanejamentoRequest(
    DateTimeOffset? DataInicioPrevista,
    DateTimeOffset? DataFimPrevista,
    string? Observacoes,
    IReadOnlyCollection<Guid>? ResponsavelIds,
    IReadOnlyCollection<PlanejamentoMaterialRequest>? Materiais);
