using LabPat.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Usuario> Usuarios => Set<Usuario>();
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

        // Email único por Usuário
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}
