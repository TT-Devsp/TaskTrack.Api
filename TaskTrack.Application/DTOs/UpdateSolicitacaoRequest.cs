namespace TaskTrack.Application.DTOs;

public sealed record UpdateSolicitacaoRequest(
    string Titulo,
    string? Descricao,
    string Localizacao);
