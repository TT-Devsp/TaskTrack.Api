using TaskTrack.Application.DTOs;
using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Mappers;

public static class ExecucaoMapper
{
    public static ExecucaoResponse ToResponse(Execucao execucao)
    {
        return new ExecucaoResponse(
            execucao.Id,
            execucao.SolicitacaoId,
            execucao.Status,
            execucao.DataInicioReal,
            execucao.DataFimReal,
            execucao.AtualizadoEm,
            execucao.AtualizadoPorId,
            execucao.ObservacaoAtualizacao);
    }
}
