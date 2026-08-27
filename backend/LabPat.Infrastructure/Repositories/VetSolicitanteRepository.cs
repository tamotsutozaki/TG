using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class VetSolicitanteRepository(AppDbContext context)
    : RepositoryBase<VetSolicitante>(context), IVetSolicitanteRepository
{
    public async Task<VetSolicitante?> GetByCrmvAsync(string numero, string estado) =>
        await Context.VetsSolicitantes.FirstOrDefaultAsync(v =>
            v.CrmvNumero == numero && v.CrmvEstado == estado);
}
