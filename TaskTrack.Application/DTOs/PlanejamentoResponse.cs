namespace TaskTrack.Application.DTOs;

public sealed record PlanejamentoResponse(
    Guid Id,
    Guid SolicitacaoId,
    DateTimeOffset? DataInicioPrevista,
    DateTimeOffset? DataFimPrevista,
    string? Observacoes,
    IReadOnlyCollection<PlanejamentoResponsavelResponse> Responsaveis,
    IReadOnlyCollection<PlanejamentoMaterialResponse> Materiais);