using LabPat.Domain.Enums;

namespace LabPat.Application.Features.Veterinarios;

// Pedido de vínculo de um veterinário, na visão do laboratório
public record VinculoVeterinarioDto(
    int Id,
    int VeterinarioId,
    string Nome,
    string Email,
    string CrmvNumero,
    string CrmvEstado,
    string? Telefone,
    StatusVinculo Status,
    DateTime SolicitadoEm,
    DateTime? RespondidoEm);
