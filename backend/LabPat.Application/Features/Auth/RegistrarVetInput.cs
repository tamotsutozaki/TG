namespace LabPat.Application.Features.Auth;

public record RegistrarVetInput(
    string Nome,
    string Email,
    string Senha,
    string CrmvNumero,
    string CrmvEstado,
    string? Telefone);
