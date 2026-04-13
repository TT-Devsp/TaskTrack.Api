using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTrack.Api.Domain.Entities;
using TaskTrack.Api.Identity;

namespace TaskTrack.Api.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<LocalPai> LocaisPai => Set<LocalPai>();
    public DbSet<LocalFilho> LocaisFilho => Set<LocalFilho>();
    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();
    public DbSet<AprovacaoSolicitacao> AprovacoesSolicitacao => Set<AprovacaoSolicitacao>();
    public DbSet<Planejamento> Planejamentos => Set<Planejamento>();
    public DbSet<PlanejamentoResponsavel> PlanejamentoResponsaveis => Set<PlanejamentoResponsavel>();
    public DbSet<PlanejamentoMaterial> PlanejamentoMateriais => Set<PlanejamentoMaterial>();
    public DbSet<Execucao> Execucoes => Set<Execucao>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<LocalPai>(entity =>
        {
            entity.ToTable("local_pai");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Endereco).HasColumnName("endereco").HasMaxLength(300);
        });

        builder.Entity<LocalFilho>(entity =>
        {
            entity.ToTable("local_filho");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.LocalPaiId).HasColumnName("local_pai_id").IsRequired();
            entity.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();

            entity.HasAlternateKey(x => new { x.Id, x.LocalPaiId });

            entity.HasOne(x => x.LocalPai)
                .WithMany(x => x.LocaisFilho)
                .HasForeignKey(x => x.LocalPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.LocalPaiId);
            entity.HasIndex(x => new { x.LocalPaiId, x.Nome }).IsUnique();
        });

        builder.Entity<Solicitacao>(entity =>
        {
            entity.ToTable("solicitacoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descricao).HasColumnName("descricao");
            entity.Property(x => x.Status).HasColumnName("status").IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").IsRequired();
            entity.Property(x => x.LocalPaiId).HasColumnName("local_pai_id").IsRequired();
            entity.Property(x => x.LocalFilhoId).HasColumnName("local_filho_id");
            entity.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();

            entity.HasOne(x => x.LocalPai)
                .WithMany(x => x.Solicitacoes)
                .HasForeignKey(x => x.LocalPaiId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.LocalFilho)
                .WithMany(x => x.Solicitacoes)
                .HasForeignKey(x => new { x.LocalFilhoId, x.LocalPaiId })
                .HasPrincipalKey(x => new { x.Id, x.LocalPaiId })
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.LocalPaiId);
            entity.HasIndex(x => x.LocalFilhoId);
            entity.HasIndex(x => x.SolicitanteId);
        });

        builder.Entity<AprovacaoSolicitacao>(entity =>
        {
            entity.ToTable("aprovacoes_solicitacao");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SolicitacaoId).HasColumnName("solicitacao_id").IsRequired();
            entity.Property(x => x.GestorId).HasColumnName("gestor_id").IsRequired();
            entity.Property(x => x.Aprovado).HasColumnName("aprovado").IsRequired();
            entity.Property(x => x.Observacao).HasColumnName("observacao");
            entity.Property(x => x.DataAprovacao).HasColumnName("data_aprovacao");

            entity.HasOne(x => x.Solicitacao)
                .WithMany(x => x.Aprovacoes)
                .HasForeignKey(x => x.SolicitacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.SolicitacaoId);
            entity.HasIndex(x => x.GestorId);
        });

        builder.Entity<Planejamento>(entity =>
        {
            entity.ToTable("planejamentos");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SolicitacaoId).HasColumnName("solicitacao_id").IsRequired();
            entity.Property(x => x.DataInicioPrevista).HasColumnName("data_inicio_prevista");
            entity.Property(x => x.DataFimPrevista).HasColumnName("data_fim_prevista");
            entity.Property(x => x.Observacoes).HasColumnName("observacoes");

            entity.HasOne(x => x.Solicitacao)
                .WithMany(x => x.Planejamentos)
                .HasForeignKey(x => x.SolicitacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.SolicitacaoId);
        });

        builder.Entity<PlanejamentoResponsavel>(entity =>
        {
            entity.ToTable("planejamento_responsaveis");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.PlanejamentoId).HasColumnName("planejamento_id").IsRequired();
            entity.Property(x => x.UsuarioId).HasColumnName("usuario_id").IsRequired();

            entity.HasOne(x => x.Planejamento)
                .WithMany(x => x.Responsaveis)
                .HasForeignKey(x => x.PlanejamentoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.PlanejamentoId);
            entity.HasIndex(x => x.UsuarioId);
        });

        builder.Entity<PlanejamentoMaterial>(entity =>
        {
            entity.ToTable("planejamento_materiais", tableBuilder =>
                tableBuilder.HasCheckConstraint("ck_planejamento_materiais_quantidade_positiva", "quantidade > 0"));
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.PlanejamentoId).HasColumnName("planejamento_id").IsRequired();
            entity.Property(x => x.Nome).HasColumnName("nome").HasMaxLength(150).IsRequired();
            entity.Property(x => x.Quantidade).HasColumnName("quantidade").HasPrecision(12, 2).IsRequired();

            entity.HasOne(x => x.Planejamento)
                .WithMany(x => x.Materiais)
                .HasForeignKey(x => x.PlanejamentoId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(x => x.PlanejamentoId);
        });

        builder.Entity<Execucao>(entity =>
        {
            entity.ToTable("execucoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.SolicitacaoId).HasColumnName("solicitacao_id").IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").IsRequired();
            entity.Property(x => x.DataInicioReal).HasColumnName("data_inicio_real");
            entity.Property(x => x.DataFimReal).HasColumnName("data_fim_real");
            entity.Property(x => x.AtualizadoEm).HasColumnName("atualizado_em").IsRequired();
            entity.Property(x => x.AtualizadoPorId).HasColumnName("atualizado_por_id");
            entity.Property(x => x.ObservacaoAtualizacao).HasColumnName("observacao_atualizacao");

            entity.HasOne(x => x.Solicitacao)
                .WithMany(x => x.Execucoes)
                .HasForeignKey(x => x.SolicitacaoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.SolicitacaoId);
            entity.HasIndex(x => x.AtualizadoPorId);
        });

        builder.Entity<Solicitacao>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.SolicitanteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AprovacaoSolicitacao>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.GestorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PlanejamentoResponsavel>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Execucao>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(x => x.AtualizadoPorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
