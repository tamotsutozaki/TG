namespace LabPat.Application.Features.Auth;

public interface IAuthService
{
    Task<LoginResultado> LoginAsync(LoginInput input);
    Task RegistrarVetAsync(RegistrarVetInput input);
    Task<ConviteInfoDto> ValidarConviteAsync(string token);
    Task RegistrarLaboratorioAsync(RegistrarLaboratorioInput input);
}
