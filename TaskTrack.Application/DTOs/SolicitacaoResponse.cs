using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record SolicitacaoResponse(
    Guid Id,
    string Titulo,
    string? Descricao,
    string Localizacao,
    SolicitacaoStatus Status,
    Prioridade Prioridade,
    DateTime DataCriacao,
    DateTime? DataLimite,
    string SolicitanteNome,
    Guid SolicitanteId);