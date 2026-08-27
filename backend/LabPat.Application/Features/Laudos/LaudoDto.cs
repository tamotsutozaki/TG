namespace LabPat.Application.Features.Laudos;

public record LaudoDto(
    int Id,
    int SolicitacaoId,
    string CodigoSolicitacao,
    string TipoExame,
    string PacienteNome,
    string Conteudo,
    string PatologistaNome,
    DateTime EmitidoEm);
