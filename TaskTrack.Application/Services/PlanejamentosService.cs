using TaskTrack.Application.DTOs;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Mappers;
using TaskTrack.Domain.Entities;
using TaskTrack.Domain.Enums;
using TaskTrack.Domain.Interfaces;

namespace TaskTrack.Application.Services;

public sealed class PlanejamentosService : IPlanejamentosService
{
    private const int ObservacoesMaxLength = 1000;
    private const int MaterialNomeMaxLength = 150;
    private readonly IPlanejamentosRepository _planejamentosRepository;
    private readonly ISolicitacoesRepository _solicitacoesRepository;

    public PlanejamentosService(
        IPlanejamentosRepository planejamentosRepository,
        ISolicitacoesRepository solicitacoesRepository)
    {
        _planejamentosRepository = planejamentosRepository;
        _solicitacoesRepository = solicitacoesRepository;
    }

    public async Task<IReadOnlyCollection<PlanejamentoResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var planejamentos = await _planejamentosRepository.GetAllAsync(cancellationToken);
        var usuariosPorId = await BuildUsuariosLookupAsync(planejamentos, cancellationToken);
        return planejamentos.Select(p => PlanejamentoMapper.ToResponse(p, usuariosPorId)).ToList();
    }

    public async Task<PlanejamentoResponse?> GetBySolicitacaoIdAsync(Guid solicitacaoId, CancellationToken cancellationToken = default)
    {
        var planejamento = await _planejamentosRepository.GetBySolicitacaoIdAsync(solicitacaoId, cancellationToken);
        if (planejamento is null)
        {
            return null;
        }

        var usuariosPorId = await BuildUsuariosLookupAsync([planejamento], cancellationToken);
        return PlanejamentoMapper.ToResponse(planejamento, usuariosPorId);
    }

    public async Task<PlanejamentoResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var planejamento = await _planejamentosRepository.GetByIdAsync(id, cancellationToken);
        if (planejamento is null)
        {
            return null;
        }

        var usuariosPorId = await BuildUsuariosLookupAsync([planejamento], cancellationToken);
        return PlanejamentoMapper.ToResponse(planejamento, usuariosPorId);
    }

    public async Task<PlanejamentoResponse> CreateAsync(CreatePlanejamentoRequest request, CancellationToken cancellationToken = default)
    {
        ValidateDatas(request.DataInicioPrevista, request.DataFimPrevista);

        var solicitacao = await _solicitacoesRepository.GetByIdForUpdateAsync(request.SolicitacaoId, cancellationToken);
        if (solicitacao is null)
        {
            throw new KeyNotFoundException("Solicitacao informada nao foi encontrada.");
        }

        var planejamentoExistente = await _planejamentosRepository.GetBySolicitacaoIdAsync(request.SolicitacaoId, cancellationToken);
        if (planejamentoExistente is not null)
        {
            throw new InvalidOperationException("Solicitacao ja possui planejamento.");
        }

        if (solicitacao.Status != SolicitacaoStatus.EmPlanejamento)
        {
            throw new InvalidOperationException("Solicitacao precisa estar em planejamento para criar o plano.");
        }

        if (solicitacao.GestorResponsavelId.HasValue && solicitacao.GestorResponsavelId != request.GestorId)
        {
            throw new UnauthorizedAccessException("Solicitacao ja esta atribuida a outro gestor para planejamento.");
        }

        var responsavelIds = NormalizeResponsavelIds(request.ResponsavelIds);
        var usuariosResponsaveis = await ValidateAndGetResponsaveisAsync(responsavelIds, cancellationToken);

        var materiais = BuildMateriais(request.Materiais);

        var planejamento = new Planejamento
        {
            Id = Guid.NewGuid(),
            SolicitacaoId = request.SolicitacaoId,
            DataInicioPrevista = request.DataInicioPrevista,
            DataFimPrevista = request.DataFimPrevista,
            Observacoes = NormalizeObservacoes(request.Observacoes),
            Responsaveis = responsavelIds
                .Select(id => new PlanejamentoResponsavel
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = id
                })
                .ToList(),
            Materiais = materiais
        };

        solicitacao.GestorResponsavelId ??= request.GestorId;
        solicitacao.Status = SolicitacaoStatus.Planejada;

        await _planejamentosRepository.AddAsync(planejamento, cancellationToken);
        await _planejamentosRepository.SaveChangesAsync(cancellationToken);

        var persisted = await _planejamentosRepository.GetByIdAsync(planejamento.Id, cancellationToken)
            ?? throw new InvalidOperationException("Nao foi possivel recuperar o planejamento apos persistencia.");

        return PlanejamentoMapper.ToResponse(persisted, usuariosResponsaveis);
    }

    public async Task<PlanejamentoResponse> UpdateAsync(Guid id, UpdatePlanejamentoRequest request, CancellationToken cancellationToken = default)
    {
        ValidateDatas(request.DataInicioPrevista, request.DataFimPrevista);

        var planejamento = await _planejamentosRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (planejamento is null)
        {
            throw new KeyNotFoundException("Planejamento informado nao foi encontrado.");
        }

        var responsavelIds = NormalizeResponsavelIds(request.ResponsavelIds);
        var usuariosResponsaveis = await ValidateAndGetResponsaveisAsync(responsavelIds, cancellationToken);

        var materiais = BuildMateriais(request.Materiais);

        planejamento.DataInicioPrevista = request.DataInicioPrevista;
        planejamento.DataFimPrevista = request.DataFimPrevista;
        planejamento.Observacoes = NormalizeObservacoes(request.Observacoes);

        planejamento.Responsaveis.Clear();
        foreach (var responsavelId in responsavelIds)
        {
            planejamento.Responsaveis.Add(new PlanejamentoResponsavel
            {
                Id = Guid.NewGuid(),
                PlanejamentoId = planejamento.Id,
                UsuarioId = responsavelId
            });
        }

        planejamento.Materiais.Clear();
        foreach (var material in materiais)
        {
            planejamento.Materiais.Add(material);
        }

        await _planejamentosRepository.SaveChangesAsync(cancellationToken);

        var persisted = await _planejamentosRepository.GetByIdAsync(planejamento.Id, cancellationToken)
            ?? throw new InvalidOperationException("Nao foi possivel recuperar o planejamento apos atualizacao.");

        return PlanejamentoMapper.ToResponse(persisted, usuariosResponsaveis);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var planejamento = await _planejamentosRepository.GetByIdForUpdateAsync(id, cancellationToken);
        if (planejamento is null)
        {
            throw new KeyNotFoundException("Planejamento informado nao foi encontrado.");
        }

        _planejamentosRepository.Remove(planejamento);
        await _planejamentosRepository.SaveChangesAsync(cancellationToken);
    }

    private static void ValidateDatas(DateTimeOffset? dataInicioPrevista, DateTimeOffset? dataFimPrevista)
    {
        if (dataInicioPrevista.HasValue && dataFimPrevista.HasValue && dataFimPrevista < dataInicioPrevista)
        {
            throw new ArgumentException("A data fim prevista nao pode ser anterior a data inicio prevista.");
        }
    }

    private static IReadOnlyCollection<Guid> NormalizeResponsavelIds(IReadOnlyCollection<Guid>? responsavelIds)
    {
        return responsavelIds?
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToList() ?? [];
    }

    private async Task<IReadOnlyDictionary<Guid, Usuario>> ValidateAndGetResponsaveisAsync(
        IReadOnlyCollection<Guid> responsavelIds,
        CancellationToken cancellationToken)
    {
        if (responsavelIds.Count == 0)
        {
            return new Dictionary<Guid, Usuario>();
        }

        var usuarios = await _planejamentosRepository.GetUsuariosByIdsAsync(responsavelIds, cancellationToken);
        if (usuarios.Count != responsavelIds.Count)
        {
            throw new KeyNotFoundException("Um ou mais responsaveis informados nao foram encontrados.");
        }

        return usuarios.ToDictionary(x => x.Id);
    }

    private async Task<IReadOnlyDictionary<Guid, Usuario>> BuildUsuariosLookupAsync(
        IReadOnlyCollection<Planejamento> planejamentos,
        CancellationToken cancellationToken)
    {
        var ids = planejamentos
            .SelectMany(p => p.Responsaveis)
            .Select(x => x.UsuarioId)
            .Distinct()
            .ToList();

        var usuarios = await _planejamentosRepository.GetUsuariosByIdsAsync(ids, cancellationToken);
        return usuarios.ToDictionary(x => x.Id);
    }

    private static List<PlanejamentoMaterial> BuildMateriais(IReadOnlyCollection<PlanejamentoMaterialRequest>? materiais)
    {
        if (materiais is null || materiais.Count == 0)
        {
            return [];
        }

        var normalized = new List<PlanejamentoMaterial>(materiais.Count);
        foreach (var material in materiais)
        {
            var nome = material.Nome?.Trim();
            if (string.IsNullOrWhiteSpace(nome))
            {
                throw new ArgumentException("O nome do material e obrigatorio.");
            }

            if (nome.Length > MaterialNomeMaxLength)
            {
                throw new ArgumentException($"O nome do material deve ter no maximo {MaterialNomeMaxLength} caracteres.");
            }

            if (material.Quantidade <= 0)
            {
                throw new ArgumentException("A quantidade do material deve ser maior que zero.");
            }

            normalized.Add(new PlanejamentoMaterial
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                Quantidade = material.Quantidade
            });
        }

        return normalized;
    }

    private static string? NormalizeObservacoes(string? observacoes)
    {
        if (string.IsNullOrWhiteSpace(observacoes))
        {
            return null;
        }

        var value = observacoes.Trim();
        if (value.Length > ObservacoesMaxLength)
        {
            throw new ArgumentException($"As observacoes devem ter no maximo {ObservacoesMaxLength} caracteres.");
        }

        return value;
    }

}
