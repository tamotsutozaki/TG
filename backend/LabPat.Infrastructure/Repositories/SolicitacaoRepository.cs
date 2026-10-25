using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LabPat.Infrastructure.Repositories;

public class SolicitacaoRepository(AppDbContext context)
    : RepositoryBase<Solicitacao>(context), ISolicitacaoRepository
{
    public async Task<IEnumerable<Solicitacao>> GetAllComResumoAsync() =>
        await ComResumo(Context.Solicitacoes)
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync();

    public async Task<Solicitacao?> GetByIdComDetalhesAsync(int id) =>
        await ComDetalhes(Context.Solicitacoes)
            .FirstOrDefaultAsync(s => s.Id == id);

    // Consulta pública (anônima): busca em todos os laboratórios, apenas pelo código
    public async Task<Solicitacao?> GetByCodigoPublicoAsync(string codigo) =>
        await Context.Solicitacoes
            .IgnoreQueryFilters()
            .Include(s => s.TipoExame)
            .Include(s => s.Paciente)
            .Include(s => s.Laboratorio)
            .FirstOrDefaultAsync(s => s.CodigoPublico == codigo);

    // O código é único no sistema inteiro, não só no laboratório
    public async Task<bool> CodigoPublicoExisteAsync(string codigo) =>
        await Context.Solicitacoes
            .IgnoreQueryFilters()
            .AnyAsync(s => s.CodigoPublico == codigo);

    public async Task<IEnumerable<Solicitacao>> GetVisiveisAoVeterinarioAsync(AcessoVeterinario acesso) =>
        await ComResumo(Context.Solicitacoes.IgnoreQueryFilters())
            .Where(VisivelAoVeterinario(acesso))
            .OrderByDescending(s => s.CriadoEm)
            .ToListAsync();

    public async Task<Solicitacao?> GetByIdVisivelAoVeterinarioAsync(int id, AcessoVeterinario acesso) =>
        await ComDetalhes(Context.Solicitacoes.IgnoreQueryFilters())
            .Where(VisivelAoVeterinario(acesso))
            .FirstOrDefaultAsync(s => s.Id == id);

    /// <summary>
    /// O veterinário vê as solicitações que ele mesmo registrou (mesmo após o vínculo ser desfeito)
    /// e, nos laboratórios com vínculo aprovado, as cadastradas pelo laboratório com o CRMV dele.
    /// </summary>
    private static Expression<Func<Solicitacao, bool>> VisivelAoVeterinario(AcessoVeterinario acesso)
    {
        var usuarioId = acesso.UsuarioId;
        var crmvNumero = acesso.CrmvNumero;
        var crmvEstado = acesso.CrmvEstado;
        var aprovados = acesso.LaboratoriosAprovados;

        return s => s.CriadoPorId == usuarioId
                    || (aprovados.Contains(s.LaboratorioId)
                        && s.VetSolicitante.CrmvNumero == crmvNumero
                        && s.VetSolicitante.CrmvEstado == crmvEstado);
    }

    private static IQueryable<Solicitacao> ComResumo(IQueryable<Solicitacao> consulta) =>
        consulta
            .Include(s => s.TipoExame)
            .Include(s => s.Paciente)
            .Include(s => s.VetSolicitante)
            .Include(s => s.Laboratorio);

    private static IQueryable<Solicitacao> ComDetalhes(IQueryable<Solicitacao> consulta) =>
        consulta
            .Include(s => s.TipoExame).ThenInclude(t => t!.ExameInsumos)
            .Include(s => s.Paciente).ThenInclude(p => p!.Tutor)
            .Include(s => s.VetSolicitante)
            .Include(s => s.Laboratorio)
            .Include(s => s.Historico);
}
