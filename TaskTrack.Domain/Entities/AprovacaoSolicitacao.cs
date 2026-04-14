namespace TaskTrack.Domain.Entities;

public class AprovacaoSolicitacao
{
    public Guid Id { get; set; }
    public Guid SolicitacaoId { get; set; }
    public Guid GestorId { get; set; }
    public bool Aprovado { get; set; }
    public string? Observacao { get; set; }
    public DateTimeOffset? DataAprovacao { get; set; }

    public Solicitacao Solicitacao { get; set; } = null!;
}

