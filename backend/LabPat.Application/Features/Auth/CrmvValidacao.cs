using LabPat.Application.Common;
using System.Text.RegularExpressions;

namespace LabPat.Application.Features.Auth;

public static partial class CrmvValidacao
{
    [GeneratedRegex(@"^\d{1,6}$")]
    private static partial Regex NumeroRegex();

    /// <summary>
    /// Valida o formato do CRMV (não verifica se o registro existe ou está ativo).
    /// UF deve ser uma das 27 válidas; número deve ter de 1 a 6 dígitos.
    /// </summary>
    public static bool FormatoValido(string? numero, string? estado)
    {
        if (string.IsNullOrWhiteSpace(numero) || string.IsNullOrWhiteSpace(estado))
            return false;

        return Validacao.UfValida(estado) && NumeroRegex().IsMatch(numero.Trim());
    }
}
