using LabPat.Domain.Enums;

namespace LabPat.Domain.Entities;

public class Solicitacao : EntityBase
{
    public string CodigoPublico { get; set; } = string.Empty;
    public StatusSolicitacao Status { get; set; } = StatusSolicitacao.Solicitado;
    public string? DescricaoClinica { get; set; }
    public MetodoEntrada MetodoEntrada { get; set; }
    public string? ArquivoEntradaUrl { get; set; }
    public DateTime? DataEstimadaConclusao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public string? ObservacoesInternas { get; set; }

    public int PacienteId { get; set; }
    public Paciente Paciente { get; set; } = null!;

    public int VetSolicitanteId { get; set; }
    public VetSolicitante VetSolicitante { get; set; } = null!;

    public int TipoExameId { get; set; }
    public TipoExame TipoExame { get; set; } = null!;

    public Laudo? Laudo { get; set; }
    public ICollection<HistoricoStatus> Historico { get; set; } = [];
}
