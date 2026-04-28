namespace TaskTrack.Application.DTOs;

public sealed record AprovacaoResponse(
    Guid Id,
    Guid SolicitacaoId,
    Guid GestorId,
    bool Aprovado,
    string? Observacao,
    DateTimeOffset? DataAprovacao);
