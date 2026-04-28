using TaskTrack.Application.DTOs;
using TaskTrack.Domain.Entities;

namespace TaskTrack.Application.Mappers;

public static class PlanejamentoMapper
{
    public static PlanejamentoResponse ToResponse(
        Planejamento planejamento,
        IReadOnlyDictionary<Guid, Usuario> usuariosPorId)
    {
        return new PlanejamentoResponse(
            planejamento.Id,
            planejamento.SolicitacaoId,
            planejamento.DataInicioPrevista,
            planejamento.DataFimPrevista,
            planejamento.Observacoes,
            planejamento.Responsaveis
                .Select(x => MapResponsavel(x, usuariosPorId))
                .ToList(),
            planejamento.Materiais
                .Select(x => new PlanejamentoMaterialResponse(x.Nome, x.Quantidade))
                .ToList());
    }

    private static PlanejamentoResponsavelResponse MapResponsavel(
        PlanejamentoResponsavel responsavel,
        IReadOnlyDictionary<Guid, Usuario> usuariosPorId)
    {
        usuariosPorId.TryGetValue(responsavel.UsuarioId, out var usuario);

        return new PlanejamentoResponsavelResponse(
            responsavel.UsuarioId,
            usuario is null
                ? null
                : new UsuarioResponse(usuario.Id, usuario.UserName, usuario.Email));
    }
}
