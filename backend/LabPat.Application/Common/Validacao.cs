using System.Net.Mail;

namespace LabPat.Application.Common;

public static class Validacao
{
    public const int TamanhoMinimoSenha = 8;

    private static readonly HashSet<string> UnidadesFederativas =
    [
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
        "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
        "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    ];

    public static bool UfValida(string? uf) =>
        !string.IsNullOrWhiteSpace(uf) && UnidadesFederativas.Contains(uf.Trim().ToUpperInvariant());

    public static bool EmailValido(string? email) =>
        !string.IsNullOrWhiteSpace(email)
        && MailAddress.TryCreate(email.Trim(), out var endereco)
        && endereco.Address == email.Trim();

    public static bool SenhaValida(string? senha) =>
        !string.IsNullOrEmpty(senha) && senha.Length >= TamanhoMinimoSenha;

    public static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();

    /// <summary>Lança RegraNegocioException se o campo obrigatório estiver vazio.</summary>
    public static string Obrigatorio(string? valor, string campo) =>
        string.IsNullOrWhiteSpace(valor)
            ? throw new RegraNegocioException($"Informe {campo}.")
            : valor.Trim();
}
