namespace LabPat.Domain.Entities;

public class Tutor : EntityBase, ITenantEntity
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }

    public int LaboratorioId { get; set; }
    public Laboratorio Laboratorio { get; set; } = null!;

    public ICollection<Paciente> Pacientes { get; set; } = [];
}
