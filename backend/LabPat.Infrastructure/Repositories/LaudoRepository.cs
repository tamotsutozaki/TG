using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class LaudoRepository(AppDbContext context)
    : RepositoryBase<Laudo>(context), ILaudoRepository
{
    public async Task<Laudo?> GetByIdComDetalhesAsync(int id) =>
        await Context.Laudos
            .Include(l => l.Solicitacao)
                .ThenInclude(s => s!.TipoExame)
            .Include(l => l.Solicitacao)
                .ThenInclude(s => s!.Paciente)
                    .ThenInclude(p => p!.Tutor)
            .Include(l => l.Solicitacao)
                .ThenInclude(s => s!.VetSolicitante)
            .Include(l => l.EmitidoPor)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Laudo?> GetBySolicitacaoIdAsync(int solicitacaoId) =>
        await Context.Laudos
            .Include(l => l.Solicitacao).ThenInclude(s => s!.TipoExame)
            .Include(l => l.Solicitacao).ThenInclude(s => s!.Paciente).ThenInclude(p => p!.Tutor)
            .Include(l => l.Solicitacao).ThenInclude(s => s!.VetSolicitante)
            .Include(l => l.EmitidoPor)
            .FirstOrDefaultAsync(l => l.SolicitacaoId == solicitacaoId);
}
