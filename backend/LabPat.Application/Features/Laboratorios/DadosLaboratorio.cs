using LabPat.Application.Common;
using LabPat.Application.Features.Auth;
using LabPat.Domain.Entities;

namespace LabPat.Application.Features.Laboratorios;

/// <summary>
/// Dados editáveis do laboratório, já validados e normalizados.
/// Usado no cadastro por convite e no "Meu perfil".
/// </summary>
public record DadosLaboratorio(
    string Nome,
    string Endereco,
    string Cidade,
    string Uf,
    string Telefone,
    string Email,
    string ResponsavelTecnicoNome,
    string ResponsavelTecnicoCrmvNumero,
    string ResponsavelTecnicoCrmvEstado)
{
    public static DadosLaboratorio Validar(
        string? nome, string? endereco, string? cidade, string? uf, string? telefone, string? email,
        string? responsavelTecnicoNome, string? responsavelTecnicoCrmvNumero, string? responsavelTecnicoCrmvEstado)
    {
        var nomeValido = Validacao.Obrigatorio(nome, "o nome do laboratório");
        var enderecoValido = Validacao.Obrigatorio(endereco, "o endereço");
        var cidadeValida = Validacao.Obrigatorio(cidade, "a cidade");
        var telefoneValido = Validacao.Obrigatorio(telefone, "o telefone");
        var rtNome = Validacao.Obrigatorio(responsavelTecnicoNome, "o nome do responsável técnico");

        if (!Validacao.UfValida(uf))
            throw new RegraNegocioException("UF inválida.");

        if (!CrmvValidacao.FormatoValido(responsavelTecnicoCrmvNumero, responsavelTecnicoCrmvEstado))
            throw new RegraNegocioException("CRMV do responsável técnico inválido. Verifique o número e a UF.");

        return new DadosLaboratorio(
            nomeValido, enderecoValido, cidadeValida, uf!.Trim().ToUpperInvariant(), telefoneValido,
            Validacao.EmailObrigatorio(email), rtNome,
            responsavelTecnicoCrmvNumero!.Trim(), responsavelTecnicoCrmvEstado!.Trim().ToUpperInvariant());
    }

    public void AplicarEm(Laboratorio laboratorio)
    {
        laboratorio.Nome = Nome;
        laboratorio.Endereco = Endereco;
        laboratorio.Cidade = Cidade;
        laboratorio.Uf = Uf;
        laboratorio.Telefone = Telefone;
        laboratorio.Email = Email;
        laboratorio.ResponsavelTecnicoNome = ResponsavelTecnicoNome;
        laboratorio.ResponsavelTecnicoCrmvNumero = ResponsavelTecnicoCrmvNumero;
        laboratorio.ResponsavelTecnicoCrmvEstado = ResponsavelTecnicoCrmvEstado;
    }
}
