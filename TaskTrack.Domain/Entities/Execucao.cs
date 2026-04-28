using TaskTrack.Domain.Enums;

namespace TaskTrack.Domain.Entities;

public class Execucao
{
    public Guid Id { get; set; }
    public Guid SolicitacaoId { get; set; }
    public ExecucaoStatus Status { get; set; }
    public DateTimeOffset? DataInicioReal { get; set; }
    public DateTimeOffset? DataFimReal { get; set; }
    public DateTimeOffset AtualizadoEm { get; set; }
    public Guid? AtualizadoPorId { get; set; }
    public string? ObservacaoAtualizacao { get; set; }

    public Solicitacao Solicitacao { get; set; } = null!;
}

