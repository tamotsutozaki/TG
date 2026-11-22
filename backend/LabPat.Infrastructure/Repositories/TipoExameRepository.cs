using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class TipoExameRepository(AppDbContext context)
    : RepositoryBase<TipoExame>(context), ITipoExameRepository
{
    public async Task<IEnumerable<TipoExame>> GetAllAtivosAsync() =>
        await Context.TiposExame
            .Where(t => t.Ativo)
            .OrderBy(t => t.Nome)
            .ToListAsync();

    // Inativos por último, para o laboratório poder reativá-los
    public async Task<IEnumerable<TipoExame>> GetAllAsync(bool incluirInativos) =>
        await Context.TiposExame
            .Where(t => incluirInativos || t.Ativo)
            .OrderByDescending(t => t.Ativo).ThenBy(t => t.Nome)
            .ToListAsync();

    public async Task<TipoExame?> GetByIdComDetalhesAsync(int id) =>
        await Context.TiposExame
            .Include(t => t.Templates)
            .Include(t => t.ExameInsumos)
                .ThenInclude(ei => ei.Insumo)
            .FirstOrDefaultAsync(t => t.Id == id);
}
