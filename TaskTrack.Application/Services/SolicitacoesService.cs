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
        var titulo = request.Titulo?.Trim();
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O titulo da solicitacao e obrigatorio.");
        }

        if (titulo.Length > TituloMaxLength)
        {
            throw new ArgumentException($"O titulo deve ter no maximo {TituloMaxLength} caracteres.");
        }

        var localizacao = request.Localizacao?.Trim();
        if (string.IsNullOrWhiteSpace(localizacao))
        {
            throw new ArgumentException("A localizacao da solicitacao e obrigatoria.");
        }

        if (localizacao.Length > LocalizacaoMaxLength)
        {
            throw new ArgumentException($"A localizacao deve ter no maximo {LocalizacaoMaxLength} caracteres.");
        }

        var solicitanteExiste = await _solicitacoesRepository.SolicitanteExistsAsync(request.SolicitanteId, cancellationToken);
        if (!solicitanteExiste)
        {
            throw new KeyNotFoundException("Solicitante informado nao foi encontrado.");
        }

        var solicitacao = new Solicitacao
        {
            Id = Guid.NewGuid(),
            Titulo = titulo,
            Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim(),
            Localizacao = localizacao,
            Status = SolicitacaoStatus.Pendente,
            DataCriacao = DateTime.Now,
            SolicitanteId = request.SolicitanteId
        };

        await _solicitacoesRepository.AddAsync(solicitacao, cancellationToken);
        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);

        var solicitacaoPersistida = await _solicitacoesRepository.GetByIdAsync(solicitacao.Id, cancellationToken)
            ?? throw new InvalidOperationException("Nao foi possivel recuperar a solicitacao apos persistencia.");

        return SolicitacaoMapper.ToResponse(solicitacaoPersistida);
    }

    public async Task<SolicitacaoResponse> UpdateAsync(Guid id, UpdateSolicitacaoRequest request, Guid solicitanteId, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        ValidateSolicitanteOwnership(solicitacao, solicitanteId);
        await ValidateNoManagerApprovalAsync(id, cancellationToken);

        var titulo = request.Titulo?.Trim();
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O titulo da solicitacao e obrigatorio.");
        }

        if (titulo.Length > TituloMaxLength)
        {
            throw new ArgumentException($"O titulo deve ter no maximo {TituloMaxLength} caracteres.");
        }

        var localizacao = request.Localizacao?.Trim();
        if (string.IsNullOrWhiteSpace(localizacao))
        {
            throw new ArgumentException("A localizacao da solicitacao e obrigatoria.");
        }

        if (localizacao.Length > LocalizacaoMaxLength)
        {
            throw new ArgumentException($"A localizacao deve ter no maximo {LocalizacaoMaxLength} caracteres.");
        }

        solicitacao.Titulo = titulo;
        solicitacao.Descricao = string.IsNullOrWhiteSpace(request.Descricao) ? null : request.Descricao.Trim();
        solicitacao.Localizacao = localizacao;

        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);

        return SolicitacaoMapper.ToResponse(solicitacao);
    }

    public async Task DeleteAsync(Guid id, Guid solicitanteId, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        ValidateSolicitanteOwnership(solicitacao, solicitanteId);
        await ValidateNoManagerApprovalAsync(id, cancellationToken);

        _solicitacoesRepository.Remove(solicitacao);
        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SolicitacaoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var solicitacoes = await _solicitacoesRepository.GetAllAsync(cancellationToken);

        return solicitacoes
            .Select(SolicitacaoMapper.ToResponse)
            .ToList();
    }

    public async Task<SolicitacaoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdAsync(id, cancellationToken);
        if (solicitacao is null)
        {
            return null;
        }

        return SolicitacaoMapper.ToResponse(solicitacao);
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetPendentesAsync(CancellationToken cancellationToken = default)
    {
        var solicitacoes = await _solicitacoesRepository.GetPendentesAsync(cancellationToken);
        return solicitacoes.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByStatusAsync(
        SolicitacaoStatus status,
        CancellationToken cancellationToken = default)
    {
        var solicitacoes = await _solicitacoesRepository.GetByStatusAsync(status, cancellationToken);
        return solicitacoes.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    public async Task<IReadOnlyCollection<SolicitacaoComGestorResponse>> GetByGestorIdAsync(Guid gestorId, CancellationToken cancellationToken = default)
    {
        var solicitacoes = await _solicitacoesRepository.GetByGestorIdAsync(gestorId, cancellationToken);
        return solicitacoes.Select(SolicitacaoMapper.ToResponseComGestor).ToList();
    }

    private static void ValidateSolicitanteOwnership(Solicitacao solicitacao, Guid solicitanteId)
    {
        if (solicitacao.SolicitanteId != solicitanteId)
        {
            throw new UnauthorizedAccessException("Apenas o solicitante pode alterar ou excluir a solicitacao.");
        }
    }

    private async Task ValidateNoManagerApprovalAsync(Guid solicitacaoId, CancellationToken cancellationToken)
    {
        var hasGestorApproval = await _solicitacoesRepository.HasGestorApprovalAsync(solicitacaoId, cancellationToken);
        if (hasGestorApproval)
        {
            throw new InvalidOperationException("Solicitacao aprovada pelo gestor nao pode ser alterada ou excluida.");
        }
    }

}
