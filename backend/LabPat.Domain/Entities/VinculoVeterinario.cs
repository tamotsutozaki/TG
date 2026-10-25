using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class VinculoVeterinario : EntityBase, ITenantEntity
{
    public StatusVinculo Status { get; set; } = StatusVinculo.Pendente;
    public DateTime SolicitadoEm { get; set; } = DateTime.UtcNow;
    public DateTime? RespondidoEm { get; set; }

    public int VeterinarioId { get; set; }
    public Usuario Veterinario { get; set; } = null!;

    public int LaboratorioId { get; set; }
    public Laboratorio Laboratorio { get; set; } = null!;
}
