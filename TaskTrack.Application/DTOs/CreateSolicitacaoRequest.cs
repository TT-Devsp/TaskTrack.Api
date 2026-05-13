using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record CreateSolicitacaoRequest(
    string Titulo,
    string? Descricao,
    string Localizacao,
    Guid SolicitanteId,
    Prioridade Prioridade);
