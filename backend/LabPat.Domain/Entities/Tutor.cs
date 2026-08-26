namespace LabPat.Domain.Entities;

public class Tutor : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string? Email { get; set; }

    public ICollection<Paciente> Pacientes { get; set; } = [];
}
