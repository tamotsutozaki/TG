using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Solicitacoes;

public record SolicitacaoDto(
    int Id,
    string CodigoPublico,
    StatusSolicitacao Status,
    string TipoExameNome,
    string PacienteNome,
    string VetSolicitanteNome,
    DateTime DataCriacao,
    DateTime? DataEstimadaConclusao);

public record SolicitacaoDetalhadaDto(
    int Id,
    string CodigoPublico,
    StatusSolicitacao Status,
    MetodoEntrada MetodoEntrada,
    string? DescricaoClinica,
    string? ObservacoesInternas,
    string? ArquivoEntradaUrl,
    DateTime DataCriacao,
    DateTime? DataEstimadaConclusao,
    DateTime? DataConclusao,
    TipoExameResumoDto TipoExame,
    PacienteResumoDto Paciente,
    TutorResumoDto Tutor,
    VetSolicitanteResumoDto VetSolicitante,
    IEnumerable<HistoricoStatusDto> Historico);

public record TipoExameResumoDto(int Id, string Nome, int PrazoEstimadoDias);

public record PacienteResumoDto(
    int Id, string Nome, string Especie, string? Raca,
    SexoPaciente Sexo, int? IdadeAnos, int? IdadeMeses, decimal? PesoKg);

public record TutorResumoDto(int Id, string Nome, string Telefone, string? Email);

public record VetSolicitanteResumoDto(
    int Id, string Nome, string CrmvNumero, string CrmvEstado,
    string? Email, string? Telefone);

public record HistoricoStatusDto(
    StatusSolicitacao StatusAnterior,
    StatusSolicitacao StatusNovo,
    DateTime AlteradoEm,
    string? Observacao);

public record ConsultaPublicaDto(
    string CodigoPublico,
    string Status,
    string TipoExame,
    string PacienteNome,
    DateTime DataCriacao,
    DateTime? DataEstimadaConclusao);
