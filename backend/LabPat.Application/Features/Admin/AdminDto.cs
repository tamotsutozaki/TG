namespace LabPat.Application.Features.Admin;

public record GerarConviteInput(string? Descricao, int? ValidadeDias);

public record AlterarAtivoInput(bool Ativo);

// O token só é devolvido aqui, no momento da criação
public record ConviteCriadoDto(int Id, string Token, string? Descricao, DateTime ExpiraEm);

public record ConviteDto(
    int Id,
    string? Descricao,
    string Status,
    DateTime CriadoEm,
    DateTime ExpiraEm,
    DateTime? UsadoEm,
    DateTime? RevogadoEm,
    string? LaboratorioNome);

public record LaboratorioAdminDto(
    int Id,
    string Nome,
    string Cnpj,
    string Cidade,
    string Uf,
    string Telefone,
    string Email,
    string ResponsavelTecnicoNome,
    bool Ativo,
    DateTime CriadoEm);

public record VeterinarioAdminDto(
    int Id,
    string Nome,
    string Email,
    string CrmvNumero,
    string CrmvEstado,
    string? Telefone,
    bool Ativo,
    DateTime CriadoEm);
