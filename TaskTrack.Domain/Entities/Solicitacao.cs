using System.ComponentModel.DataAnnotations.Schema;
using TaskTrack.Domain.Enums;

namespace TaskTrack.Domain.Entities;

public class Solicitacao
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public SolicitacaoStatus Status { get; set; }
    public Prioridade Prioridade { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataLimite { get; set; }
    
    [Column("solicitante_id")]
    public Guid SolicitanteId { get; set; }

    // Propriedade de navegação para a sua entidade de domínio User
    [ForeignKey(nameof(SolicitanteId))]
    public virtual User Solicitante { get; set; } = null!; 

    public Guid? GestorResponsavelId { get; set; }

    public ICollection<AprovacaoSolicitacao> Aprovacoes { get; set; } = new List<AprovacaoSolicitacao>();
    public ICollection<Planejamento> Planejamentos { get; set; } = new List<Planejamento>();
    public ICollection<Execucao> Execucoes { get; set; } = new List<Execucao>();
}