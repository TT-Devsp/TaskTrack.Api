namespace TaskTrack.Application.DTOs;

public sealed record PlanejamentoResponsavelResponse(
	Guid UsuarioId,
	UsuarioResponse? Usuario);