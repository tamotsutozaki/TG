using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ISolicitacaoRepository : IRepository<Solicitacao>
{
    Task<IEnumerable<Solicitacao>> GetAllComResumoAsync();
    Task<Solicitacao?> GetByIdComDetalhesAsync(int id);
    Task<Solicitacao?> GetByCodigoPublicoAsync(string codigo);
    Task<bool> CodigoPublicoExisteAsync(string codigo);
}
