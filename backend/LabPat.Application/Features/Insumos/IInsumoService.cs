namespace LabPat.Application.Features.Insumos;

public interface IInsumoService
{
    Task<IEnumerable<InsumoDto>> GetAllAsync();
    Task<InsumoDto?> GetByIdAsync(int id);
    Task<InsumoDto> CreateAsync(CreateInsumoInput input);
    Task<InsumoDto?> UpdateAsync(int id, UpdateInsumoInput input);
    Task<bool> DeleteAsync(int id);
    Task<InsumoDto?> AjustarQuantidadeAsync(int id, AjustarQuantidadeInput input);
}
