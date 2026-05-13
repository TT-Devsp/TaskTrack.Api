using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record SolicitacaoComGestorResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Localizacao,
    SolicitacaoStatus Status,
    Prioridade Prioridade,
    DateTime DataCriacao,
    DateTime? DataLimite,
    Guid SolicitanteId,
    string SolicitanteNome,
    Guid? GestorResponsavelId);