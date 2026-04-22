using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlanejamentosController : ControllerBase
{
    private readonly IPlanejamentosService _planejamentosService;

    public PlanejamentosController(IPlanejamentosService planejamentosService)
    {
        _planejamentosService = planejamentosService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var planejamentos = await _planejamentosService.GetAllAsync(cancellationToken);
        return Ok(planejamentos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var planejamento = await _planejamentosService.GetByIdAsync(id, cancellationToken);
        return planejamento is null ? NotFound() : Ok(planejamento);
    }

    [HttpGet("por-solicitacao/{solicitacaoId:guid}")]
    public async Task<IActionResult> GetBySolicitacaoId(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var planejamento = await _planejamentosService.GetBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return planejamento is null ? NotFound() : Ok(planejamento);
    }

    [HttpPost]
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

    [HttpPut("{id:guid}")]
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

    [HttpDelete("{id:guid}")]
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