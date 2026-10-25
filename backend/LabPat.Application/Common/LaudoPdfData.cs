namespace LabPat.Application.Common;

// Datas já no horário de Brasília
public record LaudoPdfData(
    // Cabeçalho: dados do laboratório
    string LaboratorioNome,
    string LaboratorioCnpj,
    string LaboratorioEndereco,
    string LaboratorioTelefone,
    string LaboratorioEmail,
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
    // Assinatura: responsável técnico no momento da emissão
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmv);
