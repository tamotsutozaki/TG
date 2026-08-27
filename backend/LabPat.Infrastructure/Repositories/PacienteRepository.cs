using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class PacienteRepository(AppDbContext context)
    : RepositoryBase<Paciente>(context), IPacienteRepository
{
    public async Task<Paciente?> GetByNomeETutorAsync(string nome, int tutorId) =>
        await Context.Pacientes.FirstOrDefaultAsync(p =>
            p.Nome == nome && p.TutorId == tutorId);
}
