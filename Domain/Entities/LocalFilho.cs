namespace TaskTrack.Api.Domain.Entities;

public class LocalFilho
{
    public Guid Id { get; set; }
    public Guid LocalPaiId { get; set; }
    public string Nome { get; set; } = string.Empty;

    public LocalPai LocalPai { get; set; } = null!;
    public ICollection<Solicitacao> Solicitacoes { get; set; } = new List<Solicitacao>();
}
