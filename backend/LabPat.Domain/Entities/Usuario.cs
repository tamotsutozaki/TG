using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public PapelUsuario Papel { get; set; } = PapelUsuario.Patologista;
    public StatusConta StatusConta { get; set; } = StatusConta.Aprovada;

    // Preenchido apenas para usuários com papel de Veterinário
    public int? VetSolicitanteId { get; set; }
    public VetSolicitante? VetSolicitante { get; set; }

    public ICollection<Laudo> Laudos { get; set; } = [];
    public ICollection<HistoricoStatus> AlteracoesStatus { get; set; } = [];
}
