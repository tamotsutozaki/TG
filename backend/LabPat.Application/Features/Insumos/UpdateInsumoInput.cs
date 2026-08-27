namespace LabPat.Application.Features.Insumos;

public record UpdateInsumoInput(string Nome, string UnidadeMedida, decimal QuantidadeMinima, bool Ativo);
