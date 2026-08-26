namespace LabPat.Domain.Entities;

public class Laudo : EntityBase
{
    public string Conteudo { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
    public DateTime EmitidoEm { get; set; }

    public int SolicitacaoId { get; set; }
    public Solicitacao Solicitacao { get; set; } = null!;

    public int EmitidoPorId { get; set; }
    public Usuario EmitidoPor { get; set; } = null!;
}
