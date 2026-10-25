namespace LabPat.Domain.Entities;

public class Laudo : EntityBase
{
    public string Conteudo { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
    public DateTime EmitidoEm { get; set; }

    // Responsável técnico no momento da emissão: o laudo continua assinado por ele
    // mesmo que o laboratório troque de responsável depois
    public string ResponsavelTecnicoNome { get; set; } = string.Empty;
    public string ResponsavelTecnicoCrmvNumero { get; set; } = string.Empty;
    public string ResponsavelTecnicoCrmvEstado { get; set; } = string.Empty;

    public int SolicitacaoId { get; set; }
    public Solicitacao Solicitacao { get; set; } = null!;

    public int EmitidoPorId { get; set; }
    public Usuario EmitidoPor { get; set; } = null!;
}
