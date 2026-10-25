using LabPat.Application.Common;
using LabPat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LabPat.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenantContext) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Laboratorio> Laboratorios => Set<Laboratorio>();
    public DbSet<ConviteLaboratorio> ConvitesLaboratorio => Set<ConviteLaboratorio>();
    public DbSet<VinculoVeterinario> VinculosVeterinario => Set<VinculoVeterinario>();
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Paciente> Pacientes => Set<Paciente>();
    public DbSet<VetSolicitante> VetsSolicitantes => Set<VetSolicitante>();
    public DbSet<TipoExame> TiposExame => Set<TipoExame>();
    public DbSet<TemplateLaudo> TemplatesLaudo => Set<TemplateLaudo>();
    public DbSet<Insumo> Insumos => Set<Insumo>();
    public DbSet<ExameInsumo> ExameInsumos => Set<ExameInsumo>();
    public DbSet<Solicitacao> Solicitacoes => Set<Solicitacao>();
    public DbSet<Laudo> Laudos => Set<Laudo>();
    public DbSet<HistoricoStatus> HistoricoStatus => Set<HistoricoStatus>();

    // Laboratório da requisição atual. Nulo para veterinário, administrador e anônimo:
    // nesse caso os filtros abaixo não retornam nenhum registro (negado por padrão).
    private int? LaboratorioIdAtual => tenantContext.LaboratorioId;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Todas as datas são gravadas em UTC; ao ler, marca como UTC para a API devolver
        // o horário com "Z" e o navegador converter para o fuso de quem está usando
        configurationBuilder.Properties<DateTime>().HaveConversion<DataUtcConverter>();
        configurationBuilder.Properties<DateTime?>().HaveConversion<DataUtcConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Chave composta da tabela de junção ExameInsumo
        modelBuilder.Entity<ExameInsumo>()
            .HasKey(e => new { e.TipoExameId, e.InsumoId });

        // CodigoPublico único na Solicitacao
        modelBuilder.Entity<Solicitacao>()
            .HasIndex(s => s.CodigoPublico)
            .IsUnique();

        // Laudo tem relação 1:1 com Solicitacao
        modelBuilder.Entity<Laudo>()
            .HasIndex(l => l.SolicitacaoId)
            .IsUnique();

        modelBuilder.Entity<Laudo>(e =>
        {
            e.Property(l => l.ResponsavelTecnicoCrmvNumero).HasMaxLength(6);
            e.Property(l => l.ResponsavelTecnicoCrmvEstado).HasMaxLength(2);
        });

        // Email único por Usuário
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // CRMV único entre as contas ativas de veterinário
        modelBuilder.Entity<Usuario>(e =>
        {
            e.Property(u => u.CrmvNumero).HasMaxLength(6);
            e.Property(u => u.CrmvEstado).HasMaxLength(2);
            e.HasIndex(u => new { u.CrmvNumero, u.CrmvEstado })
                .IsUnique()
                .HasFilter("[CrmvNumero] IS NOT NULL AND [Ativo] = 1");
        });

        // Laboratório: CNPJ e código do link de vínculo únicos
        modelBuilder.Entity<Laboratorio>(e =>
        {
            e.Property(l => l.Cnpj).HasMaxLength(14);
            e.Property(l => l.Uf).HasMaxLength(2);
            e.Property(l => l.CodigoVinculo).HasMaxLength(8);
            e.Property(l => l.ResponsavelTecnicoCrmvNumero).HasMaxLength(6);
            e.Property(l => l.ResponsavelTecnicoCrmvEstado).HasMaxLength(2);
            e.HasIndex(l => l.Cnpj).IsUnique();
            e.HasIndex(l => l.CodigoVinculo).IsUnique();
        });

        modelBuilder.Entity<ConviteLaboratorio>(e =>
        {
            e.Property(c => c.TokenHash).HasMaxLength(64);
            e.HasIndex(c => c.TokenHash).IsUnique();
            e.Property(c => c.Versao).IsRowVersion();
            e.HasOne(c => c.CriadoPor).WithMany().HasForeignKey(c => c.CriadoPorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Um vínculo por par veterinário/laboratório
        modelBuilder.Entity<VinculoVeterinario>(e =>
        {
            e.HasIndex(v => new { v.VeterinarioId, v.LaboratorioId }).IsUnique();
            e.HasOne(v => v.Veterinario).WithMany().HasForeignKey(v => v.VeterinarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Cada laboratório tem o próprio cadastro de veterinários solicitantes, um por CRMV
        modelBuilder.Entity<VetSolicitante>(e =>
        {
            e.Property(v => v.CrmvNumero).HasMaxLength(6);
            e.Property(v => v.CrmvEstado).HasMaxLength(2);
            e.HasIndex(v => new { v.LaboratorioId, v.CrmvNumero, v.CrmvEstado }).IsUnique();
        });

        // Quem registrou a solicitação (sem cascata, para não criar múltiplos caminhos de exclusão)
        modelBuilder.Entity<Solicitacao>()
            .HasOne(s => s.CriadoPor).WithMany().HasForeignKey(s => s.CriadoPorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Precisão dos campos decimais
        modelBuilder.Entity<Paciente>()
            .Property(p => p.PesoKg)
            .HasPrecision(6, 2);

        modelBuilder.Entity<Insumo>()
            .Property(i => i.QuantidadeAtual)
            .HasPrecision(10, 3);

        modelBuilder.Entity<Insumo>()
            .Property(i => i.QuantidadeMinima)
            .HasPrecision(10, 3);

        modelBuilder.Entity<ExameInsumo>()
            .Property(e => e.QuantidadeConsumida)
            .HasPrecision(10, 3);

        // Isolamento por laboratório nas entidades que pertencem a um laboratório...
        FiltrarPorLaboratorio<TipoExame>(modelBuilder);
        FiltrarPorLaboratorio<Insumo>(modelBuilder);
        FiltrarPorLaboratorio<Tutor>(modelBuilder);
        FiltrarPorLaboratorio<Paciente>(modelBuilder);
        FiltrarPorLaboratorio<VetSolicitante>(modelBuilder);
        FiltrarPorLaboratorio<Solicitacao>(modelBuilder);
        FiltrarPorLaboratorio<VinculoVeterinario>(modelBuilder);

        // ...e nas que dependem delas
        modelBuilder.Entity<TemplateLaudo>()
            .HasQueryFilter(t => t.TipoExame.LaboratorioId == LaboratorioIdAtual);
        modelBuilder.Entity<ExameInsumo>()
            .HasQueryFilter(e => e.TipoExame.LaboratorioId == LaboratorioIdAtual);
        modelBuilder.Entity<Laudo>()
            .HasQueryFilter(l => l.Solicitacao.LaboratorioId == LaboratorioIdAtual);
        modelBuilder.Entity<HistoricoStatus>()
            .HasQueryFilter(h => h.Solicitacao.LaboratorioId == LaboratorioIdAtual);

        // Laboratórios não são excluídos (são desativados): nenhuma exclusão em cascata a partir deles
        foreach (var fk in modelBuilder.Model.GetEntityTypes()
                     .SelectMany(t => t.GetForeignKeys())
                     .Where(fk => fk.PrincipalEntityType.ClrType == typeof(Laboratorio)))
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        AplicarLaboratorio();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AplicarLaboratorio();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void FiltrarPorLaboratorio<T>(ModelBuilder modelBuilder) where T : class, ITenantEntity =>
        modelBuilder.Entity<T>().HasQueryFilter(e => e.LaboratorioId == LaboratorioIdAtual);

    /// <summary>
    /// Carimba o laboratório da requisição nos registros novos e impede gravar
    /// dados em outro laboratório ou mover registros entre laboratórios.
    /// </summary>
    private void AplicarLaboratorio()
    {
        foreach (var entry in ChangeTracker.Entries<ITenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.LaboratorioId == 0)
                {
                    entry.Entity.LaboratorioId = LaboratorioIdAtual
                        ?? throw new InvalidOperationException(
                            $"Nenhum laboratório definido para gravar {entry.Entity.GetType().Name}.");
                }
                else if (LaboratorioIdAtual is int atual && entry.Entity.LaboratorioId != atual)
                {
                    throw new InvalidOperationException("Tentativa de gravar dados em outro laboratório.");
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                var laboratorio = entry.Property(e => e.LaboratorioId);
                if (laboratorio.OriginalValue != laboratorio.CurrentValue)
                    throw new InvalidOperationException("Não é permitido mover registros entre laboratórios.");
            }
        }
    }
}

internal class DataUtcConverter() : ValueConverter<DateTime, DateTime>(
    gravar => gravar,
    ler => DateTime.SpecifyKind(ler, DateTimeKind.Utc));
