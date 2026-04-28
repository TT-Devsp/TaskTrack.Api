namespace TaskTrack.Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
}
