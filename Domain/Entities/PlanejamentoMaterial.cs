namespace TaskTrack.Api.Domain.Entities;

public class PlanejamentoMaterial
{
    public Guid Id { get; set; }
    public Guid PlanejamentoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }

    public Planejamento Planejamento { get; set; } = null!;
}
