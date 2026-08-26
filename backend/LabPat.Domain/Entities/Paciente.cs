using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class Paciente : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public SexoPaciente Sexo { get; set; }
    public int? IdadeAnos { get; set; }
    public int? IdadeMeses { get; set; }
    public decimal? PesoKg { get; set; }

    public int TutorId { get; set; }
    public Tutor Tutor { get; set; } = null!;

    public ICollection<Solicitacao> Solicitacoes { get; set; } = [];
}
