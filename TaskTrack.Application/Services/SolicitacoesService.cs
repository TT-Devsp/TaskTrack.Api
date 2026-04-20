using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Domain.Entities;
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
            Status = 0,
            DataCriacao = DateTime.Now,
            SolicitanteId = request.SolicitanteId
        };

        await _solicitacoesRepository.AddAsync(solicitacao, cancellationToken);
        await _solicitacoesRepository.SaveChangesAsync(cancellationToken);

        var solicitacaoPersistida = await _solicitacoesRepository.GetByIdAsync(solicitacao.Id, cancellationToken)
            ?? throw new InvalidOperationException("Nao foi possivel recuperar a solicitacao apos persistencia.");

        return ToResponse(solicitacaoPersistida);
    }

    public async Task<IReadOnlyCollection<SolicitacaoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var solicitacoes = await _solicitacoesRepository.GetAllAsync(cancellationToken);

        return solicitacoes
            .Select(ToResponse)
            .ToList();
    }

    public async Task<SolicitacaoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var solicitacao = await _solicitacoesRepository.GetByIdAsync(id, cancellationToken);
        if (solicitacao is null)
        {
            return null;
        }

        return ToResponse(solicitacao);
    }

    private static SolicitacaoResponse ToResponse(Solicitacao solicitacao)
    {
        return new SolicitacaoResponse(
            solicitacao.Id,
            solicitacao.Titulo,
            solicitacao.Descricao,
            solicitacao.Localizacao,
            solicitacao.Status,
            solicitacao.DataCriacao,
            solicitacao.SolicitanteId);
    }
}
