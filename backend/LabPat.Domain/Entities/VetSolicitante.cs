namespace LabPat.Domain.Entities;

public class VetSolicitante : EntityBase, ITenantEntity
{
    public string Nome { get; set; } = string.Empty;
    public string CrmvNumero { get; set; } = string.Empty;
    public string CrmvEstado { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefone { get; set; }

    public int LaboratorioId { get; set; }
    public Laboratorio Laboratorio { get; set; } = null!;

    public ICollection<Solicitacao> Solicitacoes { get; set; } = [];
}
