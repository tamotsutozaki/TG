namespace LabPat.Application.Common;

public record LaudoPdfData(
    string CodigoPublico,
    string TipoExame,
    string PacienteNome,
    string Especie,
    string? Raca,
    string Sexo,
    string? Idade,
    string? Peso,
    string TutorNome,
    string VetSolicitanteNome,
    string CrmvNumero,
    string CrmvEstado,
    DateTime DataSolicitacao,
    DateTime DataEmissao,
    string Conteudo,
    string PatologistaNome);
