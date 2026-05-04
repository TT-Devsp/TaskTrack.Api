using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Constants;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints administrativos para gerenciamento de usuários e permissões.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    /// <summary>Lista todos os usuários com suas roles.</summary>
    /// <remarks>
    /// Filtros:
    /// - role=Admin|Gestor|Tecnico|Solicitante|Visualizador
    /// </remarks>
    [HttpGet("users")]
    [ProducesResponseType(typeof(IReadOnlyCollection<UserWithRoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsers([FromQuery] string? role, CancellationToken cancellationToken)
    {
        var users = await _adminService.GetAllUsersAsync(role, cancellationToken);
        return Ok(users);
    }

    /// <summary>Obtém um usuário por ID com suas roles.</summary>
    [HttpGet("users/{id:guid}")]
    [ProducesResponseType(typeof(UserWithRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _adminService.GetUserByIdAsync(id, cancellationToken);
        return user is null 
            ? NotFound(new { message = "Usuário não encontrado" })
            : Ok(user);
    }

    /// <summary>Atualiza a role de um usuário (remove a antiga e adiciona a nova).</summary>
    /// <remarks>
    /// Roles válidas: Admin, Gestor, Tecnico, Solicitante, Visualizador
    /// Payload:
    /// {
    ///   "userId": "GUID",
    ///   "newRole": "Gestor"
    /// }
    /// </remarks>
    [HttpPost("users/role")]
    [ProducesResponseType(typeof(UserWithRoleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUserRole([FromBody] UpdateUserRoleRequest request, CancellationToken cancellationToken)
    {
        // Verifica se é o próprio Admin tentando remover a role de Admin
        var currentUserIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(currentUserIdValue, out var currentUserId))
        {
            return Unauthorized(new { message = "Usuário não autenticado" });
        }

        if (currentUserId == request.UserId && request.NewRole != Roles.Admin)
        {
            return BadRequest(new { message = "Você não pode remover sua própria role de Admin" });
        }

        try
        {
            var user = await _adminService.UpdateUserRoleAsync(request.UserId, request.NewRole, cancellationToken);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }
    }

    /// <summary>Remove um usuário (não pode remover a si mesmo).</summary>
    [HttpDelete("users/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(Guid id, CancellationToken cancellationToken)
    {
        var currentUserIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(currentUserIdValue, out var currentUserId))
        {
            return Unauthorized(new { message = "Usuário não autenticado" });
        }

        try
        {
            await _adminService.DeleteUserAsync(id, currentUserId, cancellationToken);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }
    }
}
