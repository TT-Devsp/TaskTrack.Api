using Microsoft.AspNetCore.Mvc;
using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Enums;

namespace TaskTrack.Api.Controllers;

/// <summary>
/// Endpoints de solicitacoes para criacao, consulta e gestao de status.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SolicitacoesController : ControllerBase
{
    private readonly ISolicitacoesService _solicitacoesService;

    public SolicitacoesController(ISolicitacoesService solicitacoesService)
    {
        _solicitacoesService = solicitacoesService;
    }

    /// <summary>Lista todas as solicitacoes.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetAllAsync(cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Obtem uma solicitacao por id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SolicitacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var solicitacao = await _solicitacoesService.GetByIdAsync(id, cancellationToken);
        return solicitacao is null ? NotFound() : Ok(solicitacao);
    }

    /// <summary>Lista solicitacoes pendentes (sem planejamento).</summary>
    [HttpGet("pendentes")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPendentes(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetPendentesAsync(cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes aprovadas (status EmPlanejamento).</summary>
    [HttpGet("aprovadas")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAprovadas(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.EmPlanejamento, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes em analise.</summary>
    [HttpGet("em-analise")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmAnalise(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.EmAnalise, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes em planejamento.</summary>
    [HttpGet("em-planejamento")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmPlanejamento(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.EmPlanejamento, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes planejadas.</summary>
    [HttpGet("planejadas")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlanejadas(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.Planejada, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes em andamento.</summary>
    [HttpGet("em-andamento")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetEmAndamento(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.EmAndamento, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes concluidas.</summary>
    [HttpGet("concluidas")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConcluidas(CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByStatusAsync(SolicitacaoStatus.Concluida, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Lista solicitacoes vinculadas a um gestor (responsavel pelo planejamento).</summary>
    [HttpGet("minhas/{gestorId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<SolicitacaoComGestorResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByGestorId(Guid gestorId, CancellationToken cancellationToken)
    {
        var solicitacoes = await _solicitacoesService.GetByGestorIdAsync(gestorId, cancellationToken);
        return Ok(solicitacoes);
    }

    /// <summary>Cria uma solicitacao.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "titulo": "Troca de lampadas",
    ///   "descricao": "Sala 201",
    ///   "localizacao": "Bloco A",
    ///   "solicitanteId": "GUID"
    /// }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(SolicitacaoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>Atualiza uma solicitacao do solicitante.</summary>
    /// <remarks>
    /// Payload:
    /// {
    ///   "titulo": "Troca de lampadas",
    ///   "descricao": "Sala 201",
    ///   "localizacao": "Bloco A"
    /// }
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SolicitacaoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSolicitacaoRequest request,
        [FromQuery] Guid solicitanteId,
        CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _solicitacoesService.UpdateAsync(id, request, solicitanteId, cancellationToken);
            return Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>Exclui uma solicitacao do solicitante.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, [FromQuery] Guid solicitanteId, CancellationToken cancellationToken)
    {
        try
        {
            await _solicitacoesService.DeleteAsync(id, solicitanteId, cancellationToken);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
