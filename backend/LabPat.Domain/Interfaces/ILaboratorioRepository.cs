using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ILaboratorioRepository : IRepository<Laboratorio>
{
    Task<IEnumerable<Laboratorio>> GetAllOrdenadosAsync();
    Task<IEnumerable<Laboratorio>> BuscarAtivosAsync(string? termo, int limite);
    Task<Laboratorio?> GetByCodigoVinculoAsync(string codigo);
    Task<bool> CnpjExisteAsync(string cnpj);
    Task<bool> CodigoVinculoExisteAsync(string codigo);
}
