namespace LabPat.Application.Features.TiposExame;

public interface ITipoExameService
{
    Task<IEnumerable<TipoExameDto>> GetAllAsync();
    Task<TipoExameDetalhadoDto?> GetByIdAsync(int id);
    Task<TipoExameDto> CreateAsync(CreateTipoExameInput input);
    Task<TipoExameDto?> UpdateAsync(int id, UpdateTipoExameInput input);
    Task<bool> DeleteAsync(int id);
    Task<TemplateLaudoDto?> AddTemplateAsync(int tipoExameId, CreateTemplateLaudoInput input);
    Task<ExameInsumoDto?> VincularInsumoAsync(int tipoExameId, VincularInsumoInput input);
    Task<bool> DesvincularInsumoAsync(int tipoExameId, int insumoId);
}
