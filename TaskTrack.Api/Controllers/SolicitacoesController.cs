using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;

namespace TaskTrack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolicitacoesController : ControllerBase
{
    private readonly ISolicitacoesService _solicitacoesService;

    public SolicitacoesController(ISolicitacoesService solicitacoesService)
    {
        _solicitacoesService = solicitacoesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetAllAsync(cancellationToken);
        return Ok(solicitacoes);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var solicitacao = await _solicitacoesService.GetByIdAsync(id, cancellationToken);
        return solicitacao is null ? NotFound() : Ok(solicitacao);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSolicitacaoRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _solicitacoesService.CreateAsync(request, cancellationToken);
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
}
