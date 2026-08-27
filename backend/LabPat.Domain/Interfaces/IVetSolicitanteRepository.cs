using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IVetSolicitanteRepository : IRepository<VetSolicitante>
{
    Task<VetSolicitante?> GetByCrmvAsync(string numero, string estado);
}
