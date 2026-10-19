namespace LabPat.Application.Features.Veterinarios;

public record VeterinarioContaDto(
    int Id,
    string Nome,
    string Email,
    string CrmvNumero,
    string CrmvEstado,
    string? Telefone,
    string StatusConta,
    DateTime CriadoEm);
