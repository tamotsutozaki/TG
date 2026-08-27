namespace LabPat.Application.Features.TiposExame;

public record UpdateTipoExameInput(string Nome, string? Descricao, int PrazoEstimadoDias, bool Ativo);
