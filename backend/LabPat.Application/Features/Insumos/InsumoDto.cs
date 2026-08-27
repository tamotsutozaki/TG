namespace LabPat.Application.Features.Insumos;

public record InsumoDto(
    int Id,
    string Nome,
    string UnidadeMedida,
    decimal QuantidadeAtual,
    decimal QuantidadeMinima,
    bool Ativo,
    bool EmEstoqueBaixo);
