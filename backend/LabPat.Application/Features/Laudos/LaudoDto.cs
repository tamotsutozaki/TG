namespace LabPat.Application.Features.Laudos;

public record LaudoDto(
    int Id,
    int SolicitacaoId,
    string CodigoSolicitacao,
    string TipoExame,
    string PacienteNome,
    string Conteudo,
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmv,
    DateTime EmitidoEm)
{
    // Preenchido só na emissão: insumos que não tinham a quantidade necessária em estoque
    public IReadOnlyList<string> InsumosSemEstoque { get; init; } = [];
}
