namespace LabPat.Application.Features.Perfil;

// CNPJ e e-mail de login não são editáveis: identificam a conta
public record PerfilLaboratorioDto(
    string EmailLogin,
    string Nome,
    string Cnpj,
    string Endereco,
    string Cidade,
    string Uf,
    string Telefone,
    string Email,
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmvNumero,
    string ResponsavelTecnicoCrmvEstado,
    string CodigoVinculo);

public record AtualizarPerfilLaboratorioInput(
    string Nome,
    string Endereco,
    string Cidade,
    string Uf,
    string Telefone,
    string Email,
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmvNumero,
    string ResponsavelTecnicoCrmvEstado);

// O CRMV identifica o veterinário (e o que ele pode ver), por isso não é editável
public record PerfilVeterinarioDto(
    string Email,
    string Nome,
    string CrmvNumero,
    string CrmvEstado,
    string? Telefone);

public record AtualizarPerfilVeterinarioInput(string Nome, string? Telefone);

public record AlterarSenhaInput(string SenhaAtual, string NovaSenha);
