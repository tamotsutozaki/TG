using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ISolicitacaoRepository : IRepository<Solicitacao>
{
    Task<IEnumerable<Solicitacao>> GetAllComResumoAsync();
    Task<Solicitacao?> GetByIdComDetalhesAsync(int id);
    Task<Solicitacao?> GetByCodigoPublicoAsync(string codigo);
    Task<bool> CodigoPublicoExisteAsync(string codigo);

    // Veterinário: vê o que ele mesmo registrou e, nos laboratórios que aprovaram o vínculo,
    // as solicitações cadastradas com o CRMV dele
    Task<IEnumerable<Solicitacao>> GetVisiveisAoVeterinarioAsync(AcessoVeterinario acesso);
    Task<Solicitacao?> GetByIdVisivelAoVeterinarioAsync(int id, AcessoVeterinario acesso);
}

public record AcessoVeterinario(
    int UsuarioId,
    string CrmvNumero,
    string CrmvEstado,
    IReadOnlyCollection<int> LaboratoriosAprovados);
