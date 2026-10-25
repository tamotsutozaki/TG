using LabPat.Application.Common;
using LabPat.Application.Features.Laboratorios;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Perfil;

public class PerfilService(
    IUsuarioRepository usuarioRepository,
    ILaboratorioRepository laboratorioRepository,
    IPasswordHasher passwordHasher,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IPerfilService
{
    public async Task<PerfilLaboratorioDto> GetLaboratorioAsync()
    {
        var (usuario, laboratorio) = await ObterLaboratorioAsync();
        return ToDto(usuario, laboratorio);
    }

    public async Task<PerfilLaboratorioDto> AtualizarLaboratorioAsync(AtualizarPerfilLaboratorioInput input)
    {
        var (usuario, laboratorio) = await ObterLaboratorioAsync();

        var dados = DadosLaboratorio.Validar(
            input.Nome, input.Endereco, input.Cidade, input.Uf, input.Telefone, input.Email,
            input.ResponsavelTecnicoNome, input.ResponsavelTecnicoCrmvNumero, input.ResponsavelTecnicoCrmvEstado);

        dados.AplicarEm(laboratorio);
        laboratorioRepository.Update(laboratorio);

        // O nome da conta acompanha o nome do laboratório
        usuario.Nome = laboratorio.Nome;
        usuarioRepository.Update(usuario);

        await unitOfWork.CommitAsync();
        return ToDto(usuario, laboratorio);
    }

    public async Task<PerfilVeterinarioDto> GetVeterinarioAsync() =>
        ToDto(await ObterVeterinarioAsync());

    public async Task<PerfilVeterinarioDto> AtualizarVeterinarioAsync(AtualizarPerfilVeterinarioInput input)
    {
        var usuario = await ObterVeterinarioAsync();

        usuario.Nome = Validacao.Obrigatorio(input.Nome, "o nome");
        usuario.Telefone = string.IsNullOrWhiteSpace(input.Telefone) ? null : input.Telefone.Trim();
        usuarioRepository.Update(usuario);

        await unitOfWork.CommitAsync();
        return ToDto(usuario);
    }

    public async Task AlterarSenhaAsync(AlterarSenhaInput input)
    {
        var usuario = await usuarioRepository.GetByIdAsync(currentUser.Id)
            ?? throw new AcessoNegadoException("Conta não encontrada.");

        if (!passwordHasher.Verify(input.SenhaAtual, usuario.SenhaHash))
            throw new RegraNegocioException("A senha atual está incorreta.");

        Validacao.SenhaObrigatoria(input.NovaSenha);

        usuario.SenhaHash = passwordHasher.Hash(input.NovaSenha);
        usuarioRepository.Update(usuario);
        await unitOfWork.CommitAsync();
    }

    // --- helpers ---

    private async Task<(Usuario Usuario, Laboratorio Laboratorio)> ObterLaboratorioAsync()
    {
        var usuario = await usuarioRepository.GetByIdAsync(currentUser.Id);
        if (usuario?.LaboratorioId is not int laboratorioId)
            throw new AcessoNegadoException("Acesso restrito a laboratórios.");

        var laboratorio = await laboratorioRepository.GetByIdAsync(laboratorioId)
            ?? throw new AcessoNegadoException("Laboratório não encontrado.");

        return (usuario, laboratorio);
    }

    private async Task<Usuario> ObterVeterinarioAsync()
    {
        var usuario = await usuarioRepository.GetByIdAsync(currentUser.Id);
        if (usuario is null || usuario.Papel != PapelUsuario.Veterinario)
            throw new AcessoNegadoException("Acesso restrito a veterinários.");
        return usuario;
    }

    private static PerfilLaboratorioDto ToDto(Usuario usuario, Laboratorio l) =>
        new(usuario.Email, l.Nome, CnpjValidacao.Formatar(l.Cnpj), l.Endereco, l.Cidade, l.Uf,
            l.Telefone, l.Email, l.ResponsavelTecnicoNome, l.ResponsavelTecnicoCrmvNumero,
            l.ResponsavelTecnicoCrmvEstado, l.CodigoVinculo);

    private static PerfilVeterinarioDto ToDto(Usuario u) =>
        new(u.Email, u.Nome, u.CrmvNumero ?? string.Empty, u.CrmvEstado ?? string.Empty, u.Telefone);
}
