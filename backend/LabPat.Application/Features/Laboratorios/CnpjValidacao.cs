using System.Text.RegularExpressions;

namespace LabPat.Application.Features.Laboratorios;

public static partial class CnpjValidacao
{
    // 12 caracteres alfanuméricos (CNPJ alfanumérico, emitido desde jul/2026) + 2 dígitos verificadores
    [GeneratedRegex(@"^[A-Z0-9]{12}[0-9]{2}$")]
    private static partial Regex FormatoRegex();

    private static readonly int[] PesosPrimeiroDigito = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
    private static readonly int[] PesosSegundoDigito = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    /// <summary>Remove pontuação e espaços e converte para maiúsculas.</summary>
    public static string Normalizar(string? cnpj) =>
        new((cnpj ?? string.Empty).Where(char.IsLetterOrDigit).Select(char.ToUpperInvariant).ToArray());

    /// <summary>
    /// Valida os dígitos verificadores do CNPJ, numérico ou alfanumérico.
    /// Cada caractere vale seu código ASCII menos 48 ('0'..'9' = 0..9, 'A' = 17, ..., 'Z' = 42).
    /// </summary>
    public static bool Valido(string? cnpj)
    {
        var valor = Normalizar(cnpj);
        if (!FormatoRegex().IsMatch(valor) || valor.Distinct().Count() == 1)
            return false;

        return valor[12] - '0' == CalcularDigito(valor[..12], PesosPrimeiroDigito)
            && valor[13] - '0' == CalcularDigito(valor[..13], PesosSegundoDigito);
    }

    /// <summary>Formata para exibição: XX.XXX.XXX/XXXX-XX.</summary>
    public static string Formatar(string? cnpj)
    {
        var v = Normalizar(cnpj);
        return v.Length == 14 ? $"{v[..2]}.{v[2..5]}.{v[5..8]}/{v[8..12]}-{v[12..]}" : v;
    }

    private static int CalcularDigito(string baseCnpj, int[] pesos)
    {
        var soma = 0;
        for (var i = 0; i < pesos.Length; i++)
            soma += (baseCnpj[i] - '0') * pesos[i];

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }
}
