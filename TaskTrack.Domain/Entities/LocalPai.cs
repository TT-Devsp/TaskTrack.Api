namespace TaskTrack.Domain.Entities;

public class LocalPai
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Endereco { get; set; }

    public ICollection<LocalFilho> LocaisFilho { get; set; } = new List<LocalFilho>();
    public ICollection<Solicitacao> Solicitacoes { get; set; } = new List<Solicitacao>();
}

