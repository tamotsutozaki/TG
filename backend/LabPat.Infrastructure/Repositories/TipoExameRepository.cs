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

    public async Task<TipoExame?> GetByIdComTemplatesAsync(int id) =>
        await Context.TiposExame
            .Include(t => t.Templates.OrderByDescending(tl => tl.Versao))
            .FirstOrDefaultAsync(t => t.Id == id);
}
