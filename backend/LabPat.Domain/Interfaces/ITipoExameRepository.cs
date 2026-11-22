using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ITipoExameRepository : IRepository<TipoExame>
{
    Task<IEnumerable<TipoExame>> GetAllAtivosAsync();
    Task<IEnumerable<TipoExame>> GetAllAsync(bool incluirInativos);
    Task<TipoExame?> GetByIdComDetalhesAsync(int id);
}
