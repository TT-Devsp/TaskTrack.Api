using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints de aprovacoes de solicitacoes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AprovacoesController : ControllerBase
{
    private readonly IAprovacoesService _aprovacoesService;

    public AprovacoesController(IAprovacoesService aprovacoesService)
    {
        _aprovacoesService = aprovacoesService;
    }

    /// <summary>Lista aprovacoes por solicitacao.</summary>
    [HttpGet("por-solicitacao/{solicitacaoId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<AprovacaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBySolicitacaoId(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var items = await _aprovacoesService.GetBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return Ok(items);
    }

    /// <summary>Aprova ou rejeita uma solicitacao.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "solicitacaoId": "GUID",
    ///   "gestorId": "GUID",
    ///   "aprovado": true,
    ///   "observacao": "Aprovado"
    /// }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(AprovacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateAprovacaoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _aprovacoesService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetBySolicitacaoId), new { solicitacaoId = created.SolicitacaoId }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
