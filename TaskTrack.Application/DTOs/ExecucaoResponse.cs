using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record ExecucaoResponse(
    Guid Id,
    Guid SolicitacaoId,
    ExecucaoStatus Status,
    DateTimeOffset? DataInicioReal,
    DateTimeOffset? DataFimReal,
    DateTimeOffset AtualizadoEm,
    Guid? AtualizadoPorId,
    string? ObservacaoAtualizacao);