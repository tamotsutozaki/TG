namespace LabPat.Application.Features.Admin;

public interface IAdminService
{
    Task<ConviteCriadoDto> GerarConviteAsync(GerarConviteInput input);
    Task<IEnumerable<ConviteDto>> ListarConvitesAsync();
    Task<bool> RevogarConviteAsync(int id);

    Task<IEnumerable<LaboratorioAdminDto>> ListarLaboratoriosAsync();
    Task<bool> AlterarLaboratorioAtivoAsync(int id, bool ativo);

    Task<IEnumerable<VeterinarioAdminDto>> ListarVeterinariosAsync();
    Task<bool> AlterarVeterinarioAtivoAsync(int id, bool ativo);
}
