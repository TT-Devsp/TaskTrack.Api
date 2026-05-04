using TaskTrack.Application.DTOs;

namespace TaskTrack.Application.Interfaces;

public interface IJwtService
{
    AuthResponse GenerateToken(Guid userId, string email, string nome, string role);
}
