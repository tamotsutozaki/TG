using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<bool> EmailExisteAsync(string email);
    Task<IEnumerable<Usuario>> GetVeterinariosAsync();
}
