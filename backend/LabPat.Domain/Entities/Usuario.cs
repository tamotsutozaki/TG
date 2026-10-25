using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public PapelUsuario Papel { get; set; } = PapelUsuario.Patologista;

    // Preenchido apenas para a conta de um laboratório (papel Patologista)
    public int? LaboratorioId { get; set; }
    public Laboratorio? Laboratorio { get; set; }

    // Preenchidos apenas para usuários com papel de Veterinário
    public string? CrmvNumero { get; set; }
    public string? CrmvEstado { get; set; }
    public string? Telefone { get; set; }

    public ICollection<Laudo> Laudos { get; set; } = [];
    public ICollection<HistoricoStatus> AlteracoesStatus { get; set; } = [];
}
