using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ITipoExameRepository : IRepository<TipoExame>
{
    Task<IEnumerable<TipoExame>> GetAllAtivosAsync();
    Task<TipoExame?> GetByIdComDetalhesAsync(int id);
}
