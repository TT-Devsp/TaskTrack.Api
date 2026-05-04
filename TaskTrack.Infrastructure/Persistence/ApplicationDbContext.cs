using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskTrack.Domain.Entities;
using TaskTrack.Infrastructure.Identity;

namespace TaskTrack.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();
    public DbSet<AprovacaoSolicitacao> AprovacoesSolicitacao => Set<AprovacaoSolicitacao>();
    public DbSet<Planejamento> Planejamentos => Set<Planejamento>();
    public DbSet<PlanejamentoResponsavel> PlanejamentoResponsaveis => Set<PlanejamentoResponsavel>();
    public DbSet<PlanejamentoMaterial> PlanejamentoMateriais => Set<PlanejamentoMaterial>();
    public DbSet<Execucao> Execucoes => Set<Execucao>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Solicitacao>(entity =>
        {
            entity.ToTable("solicitacoes");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("id");
            entity.Property(x => x.Titulo).HasColumnName("titulo").HasMaxLength(200).IsRequired();
            entity.Property(x => x.Descricao).HasColumnName("descricao");
            entity.Property(x => x.Localizacao).HasColumnName("localizacao").HasMaxLength(500).IsRequired();
            entity.Property(x => x.Status).HasColumnName("status").IsRequired();
            entity.Property(x => x.DataCriacao).HasColumnName("data_criacao").HasColumnType("timestamp(0) without time zone").IsRequired();
            entity.Property(x => x.SolicitanteId).HasColumnName("solicitante_id").IsRequired();
            entity.Property(x => x.GestorResponsavelId).HasColumnName("gestor_responsavel_id");
            entity.HasIndex(x => x.SolicitanteId);
            entity.HasIndex(x => x.GestorResponsavelId);
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

        builder.Entity<UserProfile>(entity =>
        {
            entity.ToTable("user_profiles");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.UserId).HasColumnName("user_id");
            entity.Property(x => x.FullName).HasColumnName("full_name").HasMaxLength(200).IsRequired();

            entity.HasOne<ApplicationUser>()
                .WithOne()
                .HasForeignKey<UserProfile>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

