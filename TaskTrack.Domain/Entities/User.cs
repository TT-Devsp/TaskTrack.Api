namespace TaskTrack.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? NormalizedUserName { get; set; }
}