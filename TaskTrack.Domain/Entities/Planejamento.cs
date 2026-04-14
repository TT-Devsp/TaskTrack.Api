namespace TaskTrack.Domain.Entities;

public class Planejamento
{
    public Guid Id { get; set; }
    public Guid SolicitacaoId { get; set; }
    public DateTimeOffset? DataInicioPrevista { get; set; }
    public DateTimeOffset? DataFimPrevista { get; set; }
    public string? Observacoes { get; set; }

    public Solicitacao Solicitacao { get; set; } = null!;
    public ICollection<PlanejamentoResponsavel> Responsaveis { get; set; } = new List<PlanejamentoResponsavel>();
    public ICollection<PlanejamentoMaterial> Materiais { get; set; } = new List<PlanejamentoMaterial>();
}

