namespace TaskTrack.Application.DTOs;

public sealed record CreateAprovacaoRequest(
    Guid SolicitacaoId,
    Guid GestorId,
    bool Aprovado,
    string? Observacao);
