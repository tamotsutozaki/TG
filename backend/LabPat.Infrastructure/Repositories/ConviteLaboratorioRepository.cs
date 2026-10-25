using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class ConviteLaboratorioRepository(AppDbContext context)
    : RepositoryBase<ConviteLaboratorio>(context), IConviteLaboratorioRepository
{
    public async Task<IEnumerable<ConviteLaboratorio>> GetAllComLaboratorioAsync() =>
        await Context.ConvitesLaboratorio
            .Include(c => c.Laboratorio)
            .OrderByDescending(c => c.CriadoEm)
            .ToListAsync();

    public async Task<ConviteLaboratorio?> GetByTokenHashAsync(string tokenHash) =>
        await Context.ConvitesLaboratorio.FirstOrDefaultAsync(c => c.TokenHash == tokenHash);
}
