using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ILaudoRepository : IRepository<Laudo>
{
    Task<Laudo?> GetByIdComDetalhesAsync(int id);
    Task<Laudo?> GetBySolicitacaoIdAsync(int solicitacaoId);
}
