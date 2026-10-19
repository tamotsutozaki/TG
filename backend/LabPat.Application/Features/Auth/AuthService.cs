using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Auth;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IVetSolicitanteRepository vetRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<LoginResultado> LoginAsync(LoginInput input)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(input.Email);

        if (usuario is null || !passwordHasher.Verify(input.Senha, usuario.SenhaHash))
            return new LoginResultado(LoginStatus.CredenciaisInvalidas);

        if (usuario.StatusConta == StatusConta.Pendente)
            return new LoginResultado(LoginStatus.ContaPendente);

        if (usuario.StatusConta == StatusConta.Recusada)
            return new LoginResultado(LoginStatus.ContaRecusada);

        var token = tokenGenerator.Generate(usuario);
        var auth = new AuthDto(token, usuario.Nome, usuario.Email, usuario.Papel.ToString());
        return new LoginResultado(LoginStatus.Sucesso, auth);
    }

    public async Task<RegistroResultado> RegistrarVetAsync(RegistrarVetInput input)
    {
        if (!CrmvValidacao.FormatoValido(input.CrmvNumero, input.CrmvEstado))
            return new RegistroResultado(RegistroStatus.CrmvInvalido);

        if (await usuarioRepository.EmailExisteAsync(input.Email))
            return new RegistroResultado(RegistroStatus.EmailJaCadastrado);

        var vet = new VetSolicitante
        {
            Nome = input.Nome,
            CrmvNumero = input.CrmvNumero.Trim(),
            CrmvEstado = input.CrmvEstado.Trim().ToUpperInvariant(),
            Email = input.Email,
            Telefone = input.Telefone
        };
        await vetRepository.AddAsync(vet);

        var usuario = new Usuario
        {
            Nome = input.Nome,
            Email = input.Email,
            SenhaHash = passwordHasher.Hash(input.Senha),
            Papel = PapelUsuario.Veterinario,
            StatusConta = StatusConta.Pendente,
            VetSolicitante = vet
        };
        await usuarioRepository.AddAsync(usuario);

        await unitOfWork.CommitAsync();

        return new RegistroResultado(RegistroStatus.Sucesso);
    }
}
