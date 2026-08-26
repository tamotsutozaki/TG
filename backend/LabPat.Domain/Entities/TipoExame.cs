namespace LabPat.Domain.Entities;

public class TipoExame : EntityBase
{
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public int PrazoEstimadoDias { get; set; }
    public bool Ativo { get; set; } = true;

    public ICollection<TemplateLaudo> Templates { get; set; } = [];
    public ICollection<ExameInsumo> ExameInsumos { get; set; } = [];
    public ICollection<Solicitacao> Solicitacoes { get; set; } = [];
}
