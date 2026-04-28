using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Mappers;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Domain.Interfaces;

namespace TaskTrack.Application.Services;

public sealed class ExecucoesService : IExecucoesService
{
    private readonly IExecucoesRepository _execucoesRepository;
    private readonly ISolicitacoesRepository _solicitacoesRepository;

    public ExecucoesService(
        IExecucoesRepository execucoesRepository,
        ISolicitacoesRepository solicitacoesRepository)
    {
        _execucoesRepository = execucoesRepository;
        _solicitacoesRepository = solicitacoesRepository;
    }

    public async Task<ExecucaoResponse> StartAsync(StartExecucaoRequest request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(request.SolicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        if (solicitacao.Status != SolicitacaoStatus.Planejada)
        {
            throw new InvalidOperationException("A solicitacao precisa estar com status Planejada para iniciar a execucao.");
        }

        var current = await _execucoesRepository.GetLatestBySolicitacaoIdAsync(request.SolicitacaoId, cancellationToken);
        if (current is not null)
        {
            throw new InvalidOperationException("Ja existe uma execucao em andamento para esta solicitacao.");
        }

        var execucao = new Execucao
        {
            Id = Guid.NewGuid(),
            SolicitacaoId = request.SolicitacaoId,
            Status = request.Status,
            DataInicioReal = DateTimeOffset.UtcNow,
            DataFimReal = null,
            AtualizadoEm = DateTimeOffset.UtcNow,
            AtualizadoPorId = request.TecnicoId,
            ObservacaoAtualizacao = NormalizeObservacao(request.Observacao)
        };

        if (execucao.Status != ExecucaoStatus.EmAndamento)
        {
            throw new ArgumentException("A execucao deve iniciar com status EmAndamento.");
        }

        solicitacao.Status = SolicitacaoStatus.EmAndamento;

        await _execucoesRepository.AddAsync(execucao, cancellationToken);
        await _execucoesRepository.SaveChangesAsync(cancellationToken);

        return ExecucaoMapper.ToResponse(execucao);
    }

    public async Task<ExecucaoResponse> UpdateStatusAsync(Guid solicitacaoId, UpdateExecucaoStatusRequest request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(solicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        var current = await _execucoesRepository.GetLatestBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        if (current is null)
        {
            throw new InvalidOperationException("Nao existe execucao iniciada para esta solicitacao.");
        }

        if (current.Status == ExecucaoStatus.Concluido)
        {
            throw new InvalidOperationException("A execucao ja foi concluida e nao pode receber novas atualizacoes.");
        }

        EnsureTransitionAllowed(current.Status, request.Status);

        var updated = new Execucao
        {
            Id = Guid.NewGuid(),
            SolicitacaoId = solicitacaoId,
            Status = request.Status,
            DataInicioReal = current.DataInicioReal ?? DateTimeOffset.UtcNow,
            DataFimReal = request.Status == ExecucaoStatus.Concluido ? DateTimeOffset.UtcNow : current.DataFimReal,
            AtualizadoEm = DateTimeOffset.UtcNow,
            AtualizadoPorId = request.TecnicoId,
            ObservacaoAtualizacao = NormalizeObservacao(request.Observacao)
        };

        if (request.Status == ExecucaoStatus.Concluido)
        {
            solicitacao.Status = SolicitacaoStatus.Concluida;
        }

        await _execucoesRepository.AddAsync(updated, cancellationToken);
        await _execucoesRepository.SaveChangesAsync(cancellationToken);

        return ExecucaoMapper.ToResponse(updated);
    }

    public async Task<ExecucaoResponse?> GetLatestBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var current = await _execucoesRepository.GetLatestBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return current is null ? null : ExecucaoMapper.ToResponse(current);
    }

    public async Task<IReadOnlyCollection<ExecucaoResponse>> GetHistoricoBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var items = await _execucoesRepository.GetHistoricoBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        return items.Select(ExecucaoMapper.ToResponse).ToList();
    }

    private static void EnsureTransitionAllowed(ExecucaoStatus currentStatus, ExecucaoStatus nextStatus)
    {
        if (currentStatus == nextStatus)
        {
            return;
        }

        var allowed = currentStatus switch
        {
            ExecucaoStatus.EmAndamento => new[]
            {
                ExecucaoStatus.Pausado,
                ExecucaoStatus.Problemas,
                ExecucaoStatus.Aguardando,
                ExecucaoStatus.Bloqueado,
                ExecucaoStatus.Concluido
            },
            ExecucaoStatus.Pausado => new[]
            {
                ExecucaoStatus.EmAndamento,
                ExecucaoStatus.Problemas,
                ExecucaoStatus.Aguardando,
                ExecucaoStatus.Bloqueado,
                ExecucaoStatus.Concluido
            },
            ExecucaoStatus.Problemas => new[]
            {
                ExecucaoStatus.EmAndamento,
                ExecucaoStatus.Pausado,
                ExecucaoStatus.Aguardando,
                ExecucaoStatus.Bloqueado,
                ExecucaoStatus.Concluido
            },
            ExecucaoStatus.Aguardando => new[]
            {
                ExecucaoStatus.EmAndamento,
                ExecucaoStatus.Pausado,
                ExecucaoStatus.Problemas,
                ExecucaoStatus.Bloqueado,
                ExecucaoStatus.Concluido
            },
            ExecucaoStatus.Bloqueado => new[]
            {
                ExecucaoStatus.EmAndamento,
                ExecucaoStatus.Pausado,
                ExecucaoStatus.Problemas,
                ExecucaoStatus.Aguardando,
                ExecucaoStatus.Concluido
            },
            _ => Array.Empty<ExecucaoStatus>()
        };

        if (!allowed.Contains(nextStatus))
        {
            throw new InvalidOperationException($"Transicao de status invalida: {currentStatus} -> {nextStatus}.");
        }
    }

    private static string? NormalizeObservacao(string? observacao)
    {
        if (string.IsNullOrWhiteSpace(observacao))
        {
            return null;
        }

        return observacao.Trim();
    }

}