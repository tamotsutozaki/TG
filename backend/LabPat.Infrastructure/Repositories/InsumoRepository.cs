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

    // Inativos por último, para o laboratório poder reativá-los
    public async Task<IEnumerable<Insumo>> GetAllAsync(bool incluirInativos) =>
        await Context.Insumos
            .Where(i => incluirInativos || i.Ativo)
            .OrderByDescending(i => i.Ativo).ThenBy(i => i.Nome)
            .ToListAsync();
}
