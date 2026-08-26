namespace LabPat.Domain.Entities;

public class Usuario : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public bool Ativo { get; set; } = true;

    public ICollection<Laudo> Laudos { get; set; } = [];
    public ICollection<HistoricoStatus> AlteracoesStatus { get; set; } = [];
}
