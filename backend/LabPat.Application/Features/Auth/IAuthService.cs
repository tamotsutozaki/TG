namespace LabPat.Application.Features.Auth;

public interface IAuthService
{
    Task<AuthDto?> LoginAsync(LoginInput input);
}
