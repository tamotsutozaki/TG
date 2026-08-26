namespace LabPat.Domain.Entities;

public class TemplateLaudo : EntityBase
{
    public string Conteudo { get; set; } = string.Empty;
    public int Versao { get; set; } = 1;

    public int TipoExameId { get; set; }
    public TipoExame TipoExame { get; set; } = null!;
}
