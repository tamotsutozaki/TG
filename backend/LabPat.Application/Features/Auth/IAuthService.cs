namespace LabPat.Application.Features.Auth;

public interface IAuthService
{
    Task<LoginResultado> LoginAsync(LoginInput input);
    Task<RegistroResultado> RegistrarVetAsync(RegistrarVetInput input);
}
