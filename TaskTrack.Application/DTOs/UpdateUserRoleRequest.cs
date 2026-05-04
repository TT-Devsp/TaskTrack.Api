namespace TaskTrack.Application.DTOs;

public class UpdateUserRoleRequest
{
    public Guid UserId { get; set; }
    public string NewRole { get; set; } = string.Empty;
}
