using TaskTrack.Application.DTOs;
using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Mappers;

public static class AprovacaoMapper
{
    public static AprovacaoResponse ToResponse(AprovacaoSolicitacao entity)
    {
        return new AprovacaoResponse(
            entity.Id,
            entity.SolicitacaoId,
            entity.GestorId,
            entity.Aprovado,
            entity.Observacao,
            entity.DataAprovacao);
    }
}
