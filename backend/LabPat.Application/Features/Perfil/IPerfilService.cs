namespace LabPat.Application.Features.Perfil;

public interface IPerfilService
{
    Task<PerfilLaboratorioDto> GetLaboratorioAsync();
    Task<PerfilLaboratorioDto> AtualizarLaboratorioAsync(AtualizarPerfilLaboratorioInput input);

    Task<PerfilVeterinarioDto> GetVeterinarioAsync();
    Task<PerfilVeterinarioDto> AtualizarVeterinarioAsync(AtualizarPerfilVeterinarioInput input);

    Task AlterarSenhaAsync(AlterarSenhaInput input);
}
