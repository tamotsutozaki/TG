using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class SolicitacaoRepository(AppDbContext context)
    : RepositoryBase<Solicitacao>(context), ISolicitacaoRepository
{
    public async Task<IEnumerable<Solicitacao>> GetAllComResumoAsync() =>
        await Context.Solicitacoes
            .Include(s => s.TipoExame)
            .Include(s => s.Paciente)
            .Include(s => s.VetSolicitante)
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync();

    public async Task<Solicitacao?> GetByIdComDetalhesAsync(int id) =>
        await Context.Solicitacoes
            .Include(s => s.TipoExame).ThenInclude(t => t!.ExameInsumos)
            .Include(s => s.Paciente).ThenInclude(p => p!.Tutor)
            .Include(s => s.VetSolicitante)
            .Include(s => s.Historico)
            .FirstOrDefaultAsync(s => s.Id == id);

    public async Task<Solicitacao?> GetByCodigoPublicoAsync(string codigo) =>
        await Context.Solicitacoes
            .Include(s => s.TipoExame)
            .Include(s => s.Paciente)
            .FirstOrDefaultAsync(s => s.CodigoPublico == codigo);

    public async Task<bool> CodigoPublicoExisteAsync(string codigo) =>
        await Context.Solicitacoes.AnyAsync(s => s.CodigoPublico == codigo);
}
