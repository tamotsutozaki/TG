using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    // Retorna também contas desativadas, para o login informar o motivo da recusa
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task<bool> CrmvEmUsoAsync(string numero, string estado);
    Task<bool> ContaAtivaAsync(int id);
    Task<IEnumerable<Usuario>> GetVeterinariosAsync();
}
