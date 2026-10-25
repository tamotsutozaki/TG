using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class LaudoRepository(AppDbContext context)
    : RepositoryBase<Laudo>(context), ILaudoRepository
{
    public async Task<Laudo?> GetByIdComDetalhesAsync(int id, bool ignorarFiltroLaboratorio = false) =>
        await ComDetalhes(ignorarFiltroLaboratorio)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Laudo?> GetBySolicitacaoIdAsync(int solicitacaoId, bool ignorarFiltroLaboratorio = false) =>
        await ComDetalhes(ignorarFiltroLaboratorio)
            .FirstOrDefaultAsync(l => l.SolicitacaoId == solicitacaoId);

    private IQueryable<Laudo> ComDetalhes(bool ignorarFiltroLaboratorio)
    {
        var laudos = ignorarFiltroLaboratorio ? Context.Laudos.IgnoreQueryFilters() : Context.Laudos;
        return laudos
            .Include(l => l.Solicitacao).ThenInclude(s => s!.TipoExame)
            .Include(l => l.Solicitacao).ThenInclude(s => s!.Paciente).ThenInclude(p => p!.Tutor)
            .Include(l => l.Solicitacao).ThenInclude(s => s!.VetSolicitante)
            .Include(l => l.Solicitacao).ThenInclude(s => s!.Laboratorio)
            .Include(l => l.EmitidoPor);
    }
}
