using TaskTrack.Application.DTOs;
using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Mappers;

public static class SolicitacaoMapper
{
    public static SolicitacaoResponse ToResponse(Solicitacao solicitacao)
    {
        return new SolicitacaoResponse(
            solicitacao.Id,
            solicitacao.Titulo,
            solicitacao.Descricao,
            solicitacao.Localizacao,
            solicitacao.Status,
            solicitacao.Prioridade,
            solicitacao.DataCriacao,
            solicitacao.DataLimite,
            solicitacao.Solicitante?.UserName ?? "Usuário Desconhecido",
            solicitacao.SolicitanteId);
    }

    public static SolicitacaoComGestorResponse ToResponseComGestor(Solicitacao solicitacao)
    {
        return new SolicitacaoComGestorResponse(
            solicitacao.Id,
            solicitacao.Titulo,
            solicitacao.Descricao,
            solicitacao.Localizacao,
            solicitacao.Status,
            solicitacao.Prioridade,
            solicitacao.DataCriacao,
            solicitacao.DataLimite,
            solicitacao.SolicitanteId,
            solicitacao.Solicitante?.UserName ?? "Usuário Desconhecido",
            solicitacao.GestorResponsavelId);
    }
}