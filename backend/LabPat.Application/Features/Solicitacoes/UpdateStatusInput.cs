using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Solicitacoes;

public record UpdateStatusInput(StatusSolicitacao NovoStatus, string? Observacao);
