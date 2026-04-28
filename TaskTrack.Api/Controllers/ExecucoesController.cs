using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints de execucao de solicitacoes planejadas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ExecucoesController : ControllerBase
{
    private readonly IExecucoesService _execucoesService;

    public ExecucoesController(IExecucoesService execucoesService)
    {
        _execucoesService = execucoesService;
    }

    /// <summary>Obtem a execucao mais recente de uma solicitacao.</summary>
    [HttpGet("{solicitacaoId:guid}")]
    [ProducesResponseType(typeof(ExecucaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatest(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var execucao = await _execucoesService.GetLatestBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return execucao is null ? NotFound() : Ok(execucao);
    }

    /// <summary>Lista o historico de execucoes de uma solicitacao.</summary>
    [HttpGet("{solicitacaoId:guid}/historico")]
    [ProducesResponseType(typeof(IReadOnlyCollection<ExecucaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorico(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var historico = await _execucoesService.GetHistoricoBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return Ok(historico);
    }

    /// <summary>Inicia a execucao de uma solicitacao planejada.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "solicitacaoId": "GUID",
    ///   "tecnicoId": "GUID",
    ///   "observacao": "Inicio",
    ///   "status": 0
    /// }
    /// </remarks>
    [HttpPost("iniciar")]
    [ProducesResponseType(typeof(ExecucaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Start([FromBody] StartExecucaoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _execucoesService.StartAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetLatest), new { solicitacaoId = created.SolicitacaoId }, created);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Atualiza o status da execucao.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "tecnicoId": "GUID",
    ///   "status": 5,
    ///   "observacao": "Concluido"
    /// }
    /// </remarks>
    [HttpPut("{solicitacaoId:guid}/status")]
    [ProducesResponseType(typeof(ExecucaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(Guid solicitacaoId, [FromBody] UpdateExecucaoStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _execucoesService.UpdateStatusAsync(solicitacaoId, request, cancellationToken);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}