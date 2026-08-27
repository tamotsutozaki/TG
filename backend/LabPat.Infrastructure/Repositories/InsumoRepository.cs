using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class InsumoRepository(AppDbContext context)
    : RepositoryBase<Insumo>(context), IInsumoRepository
{
    public async Task<IEnumerable<Insumo>> GetAllAtivosAsync() =>
        await Context.Insumos
            .Where(i => i.Ativo)
            .OrderBy(i => i.Nome)
            .ToListAsync();
}
