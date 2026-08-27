namespace LabPat.Application.Features.TiposExame;

public record TipoExameDto(int Id, string Nome, string? Descricao, int PrazoEstimadoDias, bool Ativo);

public record TipoExameDetalhadoDto(
    int Id,
    string Nome,
    string? Descricao,
    int PrazoEstimadoDias,
    bool Ativo,
    IEnumerable<TemplateLaudoDto> Templates,
    IEnumerable<ExameInsumoDto> Insumos);
