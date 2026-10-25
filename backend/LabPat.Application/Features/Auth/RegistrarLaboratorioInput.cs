namespace LabPat.Application.Features.Auth;

public record RegistrarLaboratorioInput(
    string Token,
    string Nome,
    string Cnpj,
    string Endereco,
    string Cidade,
    string Uf,
    string Telefone,
    string Email,
    string Senha,
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmvNumero,
    string ResponsavelTecnicoCrmvEstado);

public record ValidarConviteInput(string Token);
