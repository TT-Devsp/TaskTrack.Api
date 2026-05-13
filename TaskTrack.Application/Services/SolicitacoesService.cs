using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Mappers;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Domain.Interfaces;

namespace TaskTrack.Application.Services;

public sealed class SolicitacoesService : ISolicitacoesService
{
    private const int TituloMaxLength = 200;
    private const int LocalizacaoMaxLength = 500;
    private readonly ISolicitacoesRepository _solicitacoesRepository;

    public SolicitacoesService(ISolicitacoesRepository solicitacoesRepository)
    {
        _solicitacoesRepository = solicitacoesRepository;
    }

    public async Task<SolicitacaoResponse> CreateAsync(CreateSolicitacaoRequest request, CancellationToken cancellationToken = default)
    {
        var titulo = NormalizeRequired(request.Titulo, "O titulo", TituloMaxLength);
        var localizacao = NormalizeRequired(request.Localizacao, "A localização", LocalizacaoMaxLength);
        var descricao = NormalizeOptional(request.Descricao);

        if (!Enum.IsDefined(request.Prioridade))
        {
            throw new ArgumentException("Prioridade inválida.");
        }

        var solicitanteExiste = await _solicitacoesRepository.SolicitanteExistsAsync(request.SolicitanteId, cancellationToken);
        if (!solicitanteExiste) throw new KeyNotFoundException("Solicitante não encontrado.");

        var nowUtc = DateTime.UtcNow;
        var now = DateTime.SpecifyKind(nowUtc, DateTimeKind.Unspecified);

        var solicitacao = new Solicitacao
        {
            Id = Guid.NewGuid(),
            Titulo = titulo,
            Descricao = descricao,
            Localizacao = localizacao,
            Status = SolicitacaoStatus.Pendente,
            Prioridade = request.Prioridade,
            DataCriacao = now,
            DataLimite = CalcularDataLimite(request.Prioridade, nowUtc),
            SolicitanteId = request.SolicitanteId
        };

        await _solicitacoesRepository.AddAsync(solicitacao, cancellationToken);
        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);

        // Recarrega do banco para garantir que o .Include(Solicitante) funcione no Mapper
        var persisted = await _solicitacoesRepository.GetByIdAsync(solicitacao.Id, cancellationToken);
        return persisted is null
            ? SolicitacaoMapper.ToResponse(solicitacao)
            : SolicitacaoMapper.ToResponse(persisted);
    }

    public async Task<SolicitacaoResponse> UpdateAsync(Guid id, UpdateSolicitacaoRequest request, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (solicitacao is null) throw new KeyNotFoundException("Solicitacao não encontrada.");

        var titulo = NormalizeRequired(request.Titulo, "O titulo", TituloMaxLength);
        var localizacao = NormalizeRequired(request.Localizacao, "A localização", LocalizacaoMaxLength);
        var descricao = NormalizeOptional(request.Descricao);

        solicitacao.Titulo = titulo;
        solicitacao.Descricao = descricao;
        solicitacao.Localizacao = localizacao;

        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);
        
        var updated = await _solicitacoesRepository.GetByIdAsync(id, cancellationToken);
        return SolicitacaoMapper.ToResponse(updated!);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (solicitacao is null) throw new KeyNotFoundException("Solicitacao não encontrada.");

        _solicitacoesRepository.Remove(solicitacao);
        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SolicitacaoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _solicitacoesRepository.GetAllAsync(cancellationToken);
        return items.Select(SolicitacaoMapper.ToResponse).ToList();
    }

    public async Task<SolicitacaoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _solicitacoesRepository.GetByIdAsync(id, cancellationToken);
        return item != null ? SolicitacaoMapper.ToResponse(item) : null;
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetPendentesAsync(CancellationToken cancellationToken = default)
    {
        var items = await _solicitacoesRepository.GetPendentesAsync(cancellationToken);
        return items.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByStatusAsync(SolicitacaoStatus status, CancellationToken cancellationToken = default)
    {
        var items = await _solicitacoesRepository.GetByStatusAsync(status, cancellationToken);
        return items.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByGestorIdAsync(Guid gestorId, CancellationToken cancellationToken = default)
    {
        var items = await _solicitacoesRepository.GetByGestorIdAsync(gestorId, cancellationToken);
        return items.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    private static DateTime CalcularDataLimite(Prioridade prioridade, DateTime baseDate)
    {
        var hoje = baseDate;
        return prioridade switch {
            Prioridade.MuitoAlta => hoje.AddDays(15),
            Prioridade.Alta => hoje.AddMonths(1),
            Prioridade.Media => hoje.AddMonths(2),
            _ => hoje.AddMonths(3)
        };
    }

    private static string NormalizeRequired(string? value, string fieldLabel, int maxLength)
    {
        var trimmed = value?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new ArgumentException($"{fieldLabel} é obrigatório.");
        }

        if (trimmed.Length > maxLength)
        {
            throw new ArgumentException($"{fieldLabel} deve ter no máximo {maxLength} caracteres.");
        }

        return trimmed;
    }

    private static string? NormalizeOptional(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? null : trimmed;
    }
}