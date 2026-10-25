using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ILaudoRepository : IRepository<Laudo>
{
    // ignorarFiltroLaboratorio: usado apenas no acesso do veterinário, após checar a permissão
    Task<Laudo?> GetByIdComDetalhesAsync(int id, bool ignorarFiltroLaboratorio = false);
    Task<Laudo?> GetBySolicitacaoIdAsync(int solicitacaoId, bool ignorarFiltroLaboratorio = false);
}
