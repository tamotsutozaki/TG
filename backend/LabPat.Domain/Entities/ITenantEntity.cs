namespace LabPat.Domain.Entities;

/// <summary>
/// Entidade que pertence a um laboratório. O AppDbContext filtra essas entidades
/// pelo laboratório do usuário logado e carimba o laboratório nas inclusões.
/// </summary>
public interface ITenantEntity
{
    int LaboratorioId { get; set; }
}
