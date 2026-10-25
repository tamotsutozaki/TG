using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class VinculoVeterinarioRepository(AppDbContext context)
    : RepositoryBase<VinculoVeterinario>(context), IVinculoVeterinarioRepository
{
    public async Task<IEnumerable<VinculoVeterinario>> GetDoLaboratorioAsync() =>
        await Context.VinculosVeterinario
            .Include(v => v.Veterinario)
            .OrderBy(v => v.Status == StatusVinculo.Pendente ? 0 : 1)
            .ThenByDescending(v => v.SolicitadoEm)
            .ToListAsync();

    public async Task<VinculoVeterinario?> GetByIdComVeterinarioAsync(int id) =>
        await Context.VinculosVeterinario
            .Include(v => v.Veterinario)
            .FirstOrDefaultAsync(v => v.Id == id);

    // As consultas abaixo atravessam laboratórios, sempre restritas ao próprio veterinário

    public async Task<IEnumerable<VinculoVeterinario>> GetDoVeterinarioAsync(int veterinarioId) =>
        await Context.VinculosVeterinario
            .IgnoreQueryFilters()
            .Include(v => v.Laboratorio)
            .Where(v => v.VeterinarioId == veterinarioId)
            .OrderByDescending(v => v.SolicitadoEm)
            .ToListAsync();

    public async Task<VinculoVeterinario?> GetDoVeterinarioNoLaboratorioAsync(int veterinarioId, int laboratorioId) =>
        await Context.VinculosVeterinario
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(v => v.VeterinarioId == veterinarioId && v.LaboratorioId == laboratorioId);

    public async Task<IReadOnlyCollection<int>> GetLaboratoriosAprovadosAsync(int veterinarioId) =>
        await Context.VinculosVeterinario
            .IgnoreQueryFilters()
            .Where(v => v.VeterinarioId == veterinarioId
                        && v.Status == StatusVinculo.Aprovado
                        && v.Laboratorio.Ativo)
            .Select(v => v.LaboratorioId)
            .ToListAsync();
}
