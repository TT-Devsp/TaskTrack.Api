namespace TaskTrack.Application.DTOs;

public sealed record SolicitacaoResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Localizacao,
    int Status,
    DateTime DataCriacao,
    Guid SolicitanteId);
