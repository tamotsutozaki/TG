using System.Security.Cryptography;
using System.Text;

namespace LabPat.Application.Common;

public static class GeradorCodigo
{
    // Sem caracteres ambíguos (0/O, 1/I)
    private const string Alfabeto = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    /// <summary>Código curto e legível, para ser digitado ou compartilhado (ex.: código público).</summary>
    public static string GerarCodigo(int tamanho) =>
        RandomNumberGenerator.GetString(Alfabeto, tamanho);

    /// <summary>Token longo (256 bits) para links de uso único, como os convites.</summary>
    public static string GerarToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');

    /// <summary>Hash SHA-256 (hex) do token; é o que fica guardado no banco.</summary>
    public static string HashToken(string token) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token.Trim())));
}
