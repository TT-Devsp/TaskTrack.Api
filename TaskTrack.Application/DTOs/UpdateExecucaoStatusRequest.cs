using TaskTrack.Domain.Enums;

namespace TaskTrack.Application.DTOs;

public sealed record UpdateExecucaoStatusRequest(
    Guid TecnicoId,
    ExecucaoStatus Status,
    string? Observacao);