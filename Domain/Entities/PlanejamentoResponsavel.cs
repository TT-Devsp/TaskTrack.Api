namespace TaskTrack.Api.Domain.Entities;

public class PlanejamentoResponsavel
{
    public Guid Id { get; set; }
    public Guid PlanejamentoId { get; set; }
    public Guid UsuarioId { get; set; }

    public Planejamento Planejamento { get; set; } = null!;
}
