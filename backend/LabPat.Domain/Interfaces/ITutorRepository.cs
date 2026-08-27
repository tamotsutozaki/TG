using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface ITutorRepository : IRepository<Tutor>
{
    Task<Tutor?> GetByTelefoneAsync(string telefone);
}
