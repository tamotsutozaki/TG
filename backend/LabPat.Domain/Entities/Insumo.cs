namespace LabPat.Domain.Entities;

public class Insumo : EntityBase, ITenantEntity
{
    public string Nome { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal QuantidadeAtual { get; set; }
    public decimal QuantidadeMinima { get; set; }
    public bool Ativo { get; set; } = true;

    public int LaboratorioId { get; set; }
    public Laboratorio Laboratorio { get; set; } = null!;

    public ICollection<ExameInsumo> ExameInsumos { get; set; } = [];
}
