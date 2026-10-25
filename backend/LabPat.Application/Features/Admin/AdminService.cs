using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Admin;

public class AdminService(
    IConviteLaboratorioRepository conviteRepository,
    ILaboratorioRepository laboratorioRepository,
    IUsuarioRepository usuarioRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IAdminService
{
    private const int ValidadePadraoDias = 7;
    private const int ValidadeMaximaDias = 30;

    public async Task<ConviteCriadoDto> GerarConviteAsync(GerarConviteInput input)
    {
        var validade = input.ValidadeDias ?? ValidadePadraoDias;
        if (validade < 1 || validade > ValidadeMaximaDias)
            throw new RegraNegocioException($"A validade deve ser de 1 a {ValidadeMaximaDias} dias.");

        var token = GeradorCodigo.GerarToken();
        var convite = new ConviteLaboratorio
        {
            TokenHash = GeradorCodigo.HashToken(token),
            Descricao = string.IsNullOrWhiteSpace(input.Descricao) ? null : input.Descricao.Trim(),
            ExpiraEm = DateTime.UtcNow.AddDays(validade),
            CriadoPorId = currentUser.Id
        };

        await conviteRepository.AddAsync(convite);
        await unitOfWork.CommitAsync();

        return new ConviteCriadoDto(convite.Id, token, convite.Descricao, convite.ExpiraEm);
    }

    public async Task<IEnumerable<ConviteDto>> ListarConvitesAsync()
    {
        var convites = await conviteRepository.GetAllComLaboratorioAsync();
        var agora = DateTime.UtcNow;

        return convites.Select(c => new ConviteDto(
            c.Id, c.Descricao, StatusDoConvite(c, agora), c.CriadoEm, c.ExpiraEm,
            c.UsadoEm, c.RevogadoEm, c.Laboratorio?.Nome));
    }

    public async Task<bool> RevogarConviteAsync(int id)
    {
        var convite = await conviteRepository.GetByIdAsync(id);
        if (convite is null) return false;

        if (convite.UsadoEm is not null)
            throw new RegraNegocioException("Este convite já foi utilizado e não pode ser revogado.");

        convite.RevogadoEm ??= DateTime.UtcNow;
        conviteRepository.Update(convite);
        await unitOfWork.CommitAsync();
        return true;
    }

    public async Task<IEnumerable<LaboratorioAdminDto>> ListarLaboratoriosAsync()
    {
        var laboratorios = await laboratorioRepository.GetAllOrdenadosAsync();
        return laboratorios.Select(l => new LaboratorioAdminDto(
            l.Id, l.Nome, l.Cnpj, l.Cidade, l.Uf, l.Telefone, l.Email,
            l.ResponsavelTecnicoNome, l.Ativo, l.CriadoEm));
    }

    public async Task<bool> AlterarLaboratorioAtivoAsync(int id, bool ativo)
    {
        var laboratorio = await laboratorioRepository.GetByIdAsync(id);
        if (laboratorio is null) return false;

        laboratorio.Ativo = ativo;
        laboratorioRepository.Update(laboratorio);
        await unitOfWork.CommitAsync();
        return true;
    }

    public async Task<IEnumerable<VeterinarioAdminDto>> ListarVeterinariosAsync()
    {
        var veterinarios = await usuarioRepository.GetVeterinariosAsync();
        return veterinarios.Select(u => new VeterinarioAdminDto(
            u.Id, u.Nome, u.Email, u.CrmvNumero ?? string.Empty, u.CrmvEstado ?? string.Empty,
            u.Telefone, u.Ativo, u.CriadoEm));
    }

    public async Task<bool> AlterarVeterinarioAtivoAsync(int id, bool ativo)
    {
        var usuario = await usuarioRepository.GetByIdAsync(id);
        if (usuario is null || usuario.Papel != PapelUsuario.Veterinario) return false;

        // Ao reativar, o CRMV pode ter sido usado por outra conta nesse meio-tempo
        if (ativo && !usuario.Ativo
            && await usuarioRepository.CrmvEmUsoAsync(usuario.CrmvNumero!, usuario.CrmvEstado!))
            throw new ConflitoException("Outra conta ativa já usa este CRMV.");

        usuario.Ativo = ativo;
        usuarioRepository.Update(usuario);
        await unitOfWork.CommitAsync();
        return true;
    }

    private static string StatusDoConvite(ConviteLaboratorio c, DateTime agora) =>
        c.UsadoEm is not null ? "Usado"
        : c.RevogadoEm is not null ? "Revogado"
        : c.ExpiraEm <= agora ? "Expirado"
        : "Ativo";
}
