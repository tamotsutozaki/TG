using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IInsumoRepository : IRepository<Insumo>
{
    Task<IEnumerable<Insumo>> GetAllAtivosAsync();
}
