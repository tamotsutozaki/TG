using LabPat.Application.Common;
using LabPat.Application.Features.Laboratorios;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Auth;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    ILaboratorioRepository laboratorioRepository,
    IConviteLaboratorioRepository conviteRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator,
    IUnitOfWork unitOfWork) : IAuthService
{
    public async Task<LoginResultado> LoginAsync(LoginInput input)
    {
        var usuario = await usuarioRepository.GetByEmailAsync(input.Email.Trim());

        if (usuario is null || !passwordHasher.Verify(input.Senha, usuario.SenhaHash))
            return new LoginResultado(LoginStatus.CredenciaisInvalidas);

        if (!usuario.Ativo || usuario.Laboratorio is { Ativo: false })
            return new LoginResultado(LoginStatus.ContaDesativada);

        var token = tokenGenerator.Generate(usuario);
        var nome = usuario.Laboratorio?.Nome ?? usuario.Nome;
        var auth = new AuthDto(token, nome, usuario.Email, usuario.Papel.ToString());
        return new LoginResultado(LoginStatus.Sucesso, auth);
    }

    public async Task RegistrarVetAsync(RegistrarVetInput input)
    {
        var nome = Validacao.Obrigatorio(input.Nome, "o nome");
        var email = Validacao.EmailObrigatorio(input.Email);
        Validacao.SenhaObrigatoria(input.Senha);

        if (!CrmvValidacao.FormatoValido(input.CrmvNumero, input.CrmvEstado))
            throw new RegraNegocioException("CRMV inválido. Verifique o número e a UF.");

        var crmvNumero = input.CrmvNumero.Trim();
        var crmvEstado = input.CrmvEstado.Trim().ToUpperInvariant();

        if (await usuarioRepository.EmailExisteAsync(email))
            throw new ConflitoException("Já existe um cadastro com este e-mail.");

        if (await usuarioRepository.CrmvEmUsoAsync(crmvNumero, crmvEstado))
            throw new ConflitoException("Já existe uma conta de veterinário com este CRMV.");

        // A conta nasce ativa; o acesso aos dados depende do vínculo aprovado por cada laboratório
        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            SenhaHash = passwordHasher.Hash(input.Senha),
            Papel = PapelUsuario.Veterinario,
            CrmvNumero = crmvNumero,
            CrmvEstado = crmvEstado,
            Telefone = string.IsNullOrWhiteSpace(input.Telefone) ? null : input.Telefone.Trim()
        };
        await usuarioRepository.AddAsync(usuario);
        await unitOfWork.CommitAsync();
    }

    public async Task<ConviteInfoDto> ValidarConviteAsync(string token)
    {
        var convite = await ObterConviteValidoAsync(token);
        return new ConviteInfoDto(convite.Descricao, convite.ExpiraEm);
    }

    public async Task RegistrarLaboratorioAsync(RegistrarLaboratorioInput input)
    {
        var convite = await ObterConviteValidoAsync(input.Token);

        // O e-mail informado é o login e também o contato impresso no laudo
        var dados = DadosLaboratorio.Validar(
            input.Nome, input.Endereco, input.Cidade, input.Uf, input.Telefone, input.Email,
            input.ResponsavelTecnicoNome, input.ResponsavelTecnicoCrmvNumero, input.ResponsavelTecnicoCrmvEstado);

        var cnpj = CnpjValidacao.Normalizar(input.Cnpj);
        if (!CnpjValidacao.Valido(cnpj))
            throw new RegraNegocioException("CNPJ inválido.");

        Validacao.SenhaObrigatoria(input.Senha);

        if (await laboratorioRepository.CnpjExisteAsync(cnpj))
            throw new ConflitoException("Já existe um laboratório cadastrado com este CNPJ.");

        if (await usuarioRepository.EmailExisteAsync(dados.Email))
            throw new ConflitoException("Já existe um cadastro com este e-mail.");

        var laboratorio = new Laboratorio
        {
            Cnpj = cnpj,
            CodigoVinculo = await GerarCodigoVinculoAsync()
        };
        dados.AplicarEm(laboratorio);
        await laboratorioRepository.AddAsync(laboratorio);

        // O laboratório tem um único acesso, compartilhado por quem trabalha nele
        var usuario = new Usuario
        {
            Nome = dados.Nome,
            Email = dados.Email,
            SenhaHash = passwordHasher.Hash(input.Senha),
            Papel = PapelUsuario.Patologista,
            Laboratorio = laboratorio
        };
        await usuarioRepository.AddAsync(usuario);

        convite.UsadoEm = DateTime.UtcNow;
        convite.Laboratorio = laboratorio;

        await unitOfWork.CommitAsync();
    }

    // --- helpers ---

    private async Task<ConviteLaboratorio> ObterConviteValidoAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new RegraNegocioException("Convite inválido.");

        var convite = await conviteRepository.GetByTokenHashAsync(GeradorCodigo.HashToken(token))
            ?? throw new RegraNegocioException("Convite inválido.");

        if (convite.UsadoEm is not null)
            throw new RegraNegocioException("Este convite já foi utilizado.");
        if (convite.RevogadoEm is not null)
            throw new RegraNegocioException("Este convite foi revogado.");
        if (convite.ExpiraEm <= DateTime.UtcNow)
            throw new RegraNegocioException("Este convite expirou.");

        return convite;
    }

    private async Task<string> GerarCodigoVinculoAsync()
    {
        string codigo;
        do codigo = GeradorCodigo.GerarCodigo(8);
        while (await laboratorioRepository.CodigoVinculoExisteAsync(codigo));
        return codigo;
    }
}
