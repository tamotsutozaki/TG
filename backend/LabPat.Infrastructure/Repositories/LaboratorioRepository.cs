using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class LaboratorioRepository(AppDbContext context)
    : RepositoryBase<Laboratorio>(context), ILaboratorioRepository
{
    public async Task<IEnumerable<Laboratorio>> GetAllOrdenadosAsync() =>
        await Context.Laboratorios
            .OrderBy(l => l.Nome)
            .ToListAsync();

    public async Task<IEnumerable<Laboratorio>> BuscarAtivosAsync(string? termo, int limite)
    {
        var consulta = Context.Laboratorios.Where(l => l.Ativo);

        if (!string.IsNullOrWhiteSpace(termo))
        {
            var t = termo.Trim();
            consulta = consulta.Where(l => l.Nome.Contains(t) || l.Cidade.Contains(t));
        }

        return await consulta
            .OrderBy(l => l.Nome)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<Laboratorio?> GetByCodigoVinculoAsync(string codigo) =>
        await Context.Laboratorios.FirstOrDefaultAsync(l => l.CodigoVinculo == codigo);

    public async Task<bool> CnpjExisteAsync(string cnpj) =>
        await Context.Laboratorios.AnyAsync(l => l.Cnpj == cnpj);

    public async Task<bool> CodigoVinculoExisteAsync(string codigo) =>
        await Context.Laboratorios.AnyAsync(l => l.CodigoVinculo == codigo);
}
