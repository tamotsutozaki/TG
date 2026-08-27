namespace LabPat.Application.Features.Solicitacoes;

public record VetSolicitanteInput(
    string Nome,
    string CrmvNumero,
    string CrmvEstado,
    string? Email,
    string? Telefone);
