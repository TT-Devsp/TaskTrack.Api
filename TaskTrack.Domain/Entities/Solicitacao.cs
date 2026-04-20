namespace TaskTrack.Domain.Entities;

public class Solicitacao
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public int Status { get; set; }
    public DateTime DataCriacao { get; set; }
    public Guid SolicitanteId { get; set; }

    public ICollection<AprovacaoSolicitacao> Aprovacoes { get; set; } = new List<AprovacaoSolicitacao>();
    public ICollection<Planejamento> Planejamentos { get; set; } = new List<Planejamento>();
    public ICollection<Execucao> Execucoes { get; set; } = new List<Execucao>();
}

