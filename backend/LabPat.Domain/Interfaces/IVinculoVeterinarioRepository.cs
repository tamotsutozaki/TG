using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IVinculoVeterinarioRepository : IRepository<VinculoVeterinario>
{
    // Visão do laboratório logado (filtrada pelo laboratório)
    Task<IEnumerable<VinculoVeterinario>> GetDoLaboratorioAsync();
    Task<VinculoVeterinario?> GetByIdComVeterinarioAsync(int id);

    // Visão do veterinário (atravessa laboratórios, sempre restrita ao próprio veterinário)
    Task<IEnumerable<VinculoVeterinario>> GetDoVeterinarioAsync(int veterinarioId);
    Task<VinculoVeterinario?> GetDoVeterinarioNoLaboratorioAsync(int veterinarioId, int laboratorioId);
    Task<IReadOnlyCollection<int>> GetLaboratoriosAprovadosAsync(int veterinarioId);
}
