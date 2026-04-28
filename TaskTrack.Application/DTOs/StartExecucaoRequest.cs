using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record StartExecucaoRequest(
    Guid SolicitacaoId,
    Guid TecnicoId,
    string? Observacao,
    ExecucaoStatus Status = ExecucaoStatus.EmAndamento);
