using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record SolicitacaoComGestorResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Localizacao,
    SolicitacaoStatus Status,
    DateTime DataCriacao,
    Guid SolicitanteId,
    Guid? GestorResponsavelId);
