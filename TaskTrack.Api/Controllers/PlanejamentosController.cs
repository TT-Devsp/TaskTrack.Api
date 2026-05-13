using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Constants;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints de planejamento de solicitacoes.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Qualquer usuario autenticado
public class PlanejamentosController : ControllerBase
{
    private readonly IPlanejamentosService _planejamentosService;

    public PlanejamentosController(IPlanejamentosService planejamentosService)
    {
        _planejamentosService = planejamentosService;
    }

    /// <summary>Lista todos os planejamentos.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PlanejamentoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var planejamentos = await _planejamentosService.GetAllAsync(cancellationToken);
        return Ok(planejamentos);
    }

    /// <summary>Obtem um planejamento por id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlanejamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var planejamento = await _planejamentosService.GetByIdAsync(id, cancellationToken);
        return planejamento is null ? NotFound() : Ok(planejamento);
    }

    /// <summary>Obtem um planejamento por solicitacao.</summary>
    [HttpGet("por-solicitacao/{solicitacaoId:guid}")]
    [ProducesResponseType(typeof(PlanejamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySolicitacaoId(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var planejamento = await _planejamentosService.GetBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return planejamento is null ? NotFound() : Ok(planejamento);
    }

    /// <summary>Cria o planejamento de uma solicitacao aprovada.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "solicitacaoId": "GUID",
    ///   "gestorId": "GUID",
    ///   "dataInicioPrevista": "2026-04-29T14:03:59.2743561Z",
    ///   "dataFimPrevista": "2026-04-30T14:03:59.2799256Z",
    ///   "observacoes": "Planejamento inicial",
    ///   "responsavelIds": ["GUID"],
    ///   "materiais": [{"nome": "Lampada", "quantidade": 4}]
    /// }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(PlanejamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreatePlanejamentoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _planejamentosService.CreateAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Atualiza um planejamento.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PlanejamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePlanejamentoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _planejamentosService.UpdateAsync(id, request, cancellationToken);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Exclui um planejamento.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _planejamentosService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}