namespace LabPat.Application.Features.Vinculos;

// Operações do veterinário sobre os próprios vínculos
public interface IVinculoService
{
    Task<IEnumerable<LaboratorioBuscaDto>> BuscarLaboratoriosAsync(string? termo);
    Task<LaboratorioBuscaDto?> GetLaboratorioPorCodigoAsync(string codigo);
    Task<IEnumerable<MeuVinculoDto>> ListarMeusVinculosAsync();
    Task<MeuVinculoDto> SolicitarAsync(int laboratorioId);
}
