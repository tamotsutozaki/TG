using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IConviteLaboratorioRepository : IRepository<ConviteLaboratorio>
{
    Task<IEnumerable<ConviteLaboratorio>> GetAllComLaboratorioAsync();
    Task<ConviteLaboratorio?> GetByTokenHashAsync(string tokenHash);
}
