namespace LabPat.Domain.Entities;

public class VetSolicitante : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string CrmvNumero { get; set; } = string.Empty;
    public string CrmvEstado { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    public ICollection<Solicitacao> Solicitacoes { get; set; } = [];
}
