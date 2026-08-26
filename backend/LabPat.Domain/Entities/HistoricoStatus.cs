using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class HistoricoStatus
{
    public int Id { get; set; }
    public StatusSolicitacao StatusAnterior { get; set; }
    public StatusSolicitacao StatusNovo { get; set; }
    public string? Observacao { get; set; }
    public DateTime AlteradoEm { get; set; } = DateTime.UtcNow;

    public int SolicitacaoId { get; set; }
    public Solicitacao Solicitacao { get; set; } = null!;

    public int AlteradoPorId { get; set; }
    public Usuario AlteradoPor { get; set; } = null!;
}
