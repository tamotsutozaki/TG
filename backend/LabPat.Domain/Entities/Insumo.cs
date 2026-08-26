namespace LabPat.Domain.Entities;

public class Insumo : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string UnidadeMedida { get; set; } = string.Empty;
    public decimal QuantidadeAtual { get; set; }
    public decimal QuantidadeMinima { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<ExameInsumo> ExameInsumos { get; set; } = [];
}
