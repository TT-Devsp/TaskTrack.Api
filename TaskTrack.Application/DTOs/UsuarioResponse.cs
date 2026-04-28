namespace TaskTrack.Application.DTOs;

public sealed record UsuarioResponse(
    Guid Id,
    string? UserName,
    string? Email);
