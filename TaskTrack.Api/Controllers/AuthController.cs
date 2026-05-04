using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Constants;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints de autenticação e registro de usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Autentica um usuário e retorna token JWT.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "email": "usuario@exemplo.com",
    ///   "password": "Senha@123"
    /// }
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAsync(request, cancellationToken);
        return response is null 
            ? Unauthorized(new { message = "Email ou senha inválidos" })
            : Ok(response);
    }

    /// <summary>Registra um novo usuário (sempre como Visualizador por padrão).</summary>
    /// <remarks>
    /// O usuário é registrado com role "Visualizador". Para alterar a role, use o endpoint de admin.
    /// Payload:
    /// {
    ///   "email": "usuario@exemplo.com",
    ///   "password": "Senha@123",
    ///   "nome": "Nome do Usuario"
    /// }
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Lista todas as roles disponíveis.</summary>
    [HttpGet("roles")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetRoles()
    {
        return Ok(Roles.All);
    }
}
