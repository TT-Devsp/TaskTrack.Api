using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Mappers;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Domain.Interfaces;

namespace TaskTrack.Application.Services;

public sealed class AprovacoesService : IAprovacoesService
{
    private readonly IAprovacoesRepository _aprovacoesRepository;
    private readonly ISolicitacoesRepository _solicitacoesRepository;

    public AprovacoesService(
        IAprovacoesRepository aprovacoesRepository,
        ISolicitacoesRepository solicitacoesRepository)
    {
        _aprovacoesRepository = aprovacoesRepository;
        _solicitacoesRepository = solicitacoesRepository;
    }

    public async Task<AprovacaoResponse> CreateAsync(CreateAprovacaoRequest request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(request.SolicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        if (request.Aprovado)
        {
            solicitacao.Status = SolicitacaoStatus.EmPlanejamento;
        }

        var aprovacao = new AprovacaoSolicitacao
        {
            Id = Guid.NewGuid(),
            SolicitacaoId = request.SolicitacaoId,
            GestorId = request.GestorId,
            Aprovado = request.Aprovado,
            Observacao = string.IsNullOrWhiteSpace(request.Observacao) ? null : request.Observacao.Trim(),
            DataAprovacao = request.Aprovado ? DateTimeOffset.UtcNow : null
        };

        await _aprovacoesRepository.AddAsync(aprovacao, cancellationToken);
        await _aprovacoesRepository.SaveChangesAsync(cancellationToken);

        return AprovacaoMapper.ToResponse(aprovacao);
    }

    public async Task<IReadOnlyCollection<AprovacaoResponse>> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var items = await _aprovacoesRepository.GetBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return items.Select(AprovacaoMapper.ToResponse).ToList();
    }
}
