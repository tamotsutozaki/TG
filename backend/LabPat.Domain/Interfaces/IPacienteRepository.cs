using LabPat.Domain.Entities;

namespace LabPat.Domain.Interfaces;

public interface IPacienteRepository : IRepository<Paciente>
{
    Task<Paciente?> GetByNomeETutorAsync(string nome, int tutorId);
}
