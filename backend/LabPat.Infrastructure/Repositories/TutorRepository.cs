using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class TutorRepository(AppDbContext context)
    : RepositoryBase<Tutor>(context), ITutorRepository
{
    public async Task<Tutor?> GetByTelefoneAsync(string telefone) =>
        await Context.Tutores.FirstOrDefaultAsync(t => t.Telefone == telefone);
}
