using LabPat.Application.Common;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Auth;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator) : IAuthService
{
    public async Task<AuthDto?> LoginAsync(LoginInput input)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(input.Email);

        if (usuario is null || !passwordHasher.Verify(input.Senha, usuario.SenhaHash))
            return null;

        var token = tokenGenerator.Generate(usuario);
        return new AuthDto(token, usuario.Nome, usuario.Email);
    }
}
