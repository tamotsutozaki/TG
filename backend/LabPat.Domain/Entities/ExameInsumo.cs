namespace LabPat.Domain.Entities;

public class ExameInsumo
{
    public int TipoExameId { get; set; }
    public TipoExame TipoExame { get; set; } = null!;

    public int InsumoId { get; set; }
    public Insumo Insumo { get; set; } = null!;

    public decimal QuantidadeConsumida { get; set; }
}
