using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Solicitacoes;

public record CreateSolicitacaoInput(
    VetSolicitanteInput VetSolicitante,
    TutorInput Tutor,
    PacienteInput Paciente,
    int TipoExameId,
    string? DescricaoClinica,
    MetodoEntrada MetodoEntrada,
    string? ArquivoEntradaUrl);
