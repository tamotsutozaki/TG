using LabPat.Application.Common;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/gemini")]
[Authorize(Roles = "Patologista,Veterinario")]
public class GeminiController(IGeminiService geminiService) : ControllerBase
{
    private static readonly HashSet<string> MimeTypesPermitidos =
    [
        "image/jpeg", "image/png", "image/webp", "image/heic",
        "application/pdf",
        "audio/mp3", "audio/mpeg", "audio/wav", "audio/ogg", "audio/webm"
    ];

    private const long TamanhoMaximoBytes = 10 * 1024 * 1024; // 10 MB

    [HttpPost("extrair")]
    public async Task<IActionResult> Extrair(IFormFile arquivo)
    {
        if (arquivo.Length == 0)
            return BadRequest(new { message = "Arquivo vazio." });

        if (arquivo.Length > TamanhoMaximoBytes)
            return BadRequest(new { message = "Arquivo excede o limite de 10 MB." });

        var mimeType = arquivo.ContentType.ToLowerInvariant();
        if (!MimeTypesPermitidos.Contains(mimeType))
            return BadRequest(new { message = $"Tipo de arquivo não suportado: {mimeType}" });

        using var ms = new MemoryStream();
        await arquivo.CopyToAsync(ms);
        var bytes = ms.ToArray();

        try
        {
            var resultado = await geminiService.ExtrairDadosAsync(bytes, mimeType);
            return Ok(resultado);
        }
        catch (HttpRequestException ex) when ((int?)ex.StatusCode is 429 or 500 or 503)
        {
            return StatusCode(503, new { message = "O serviço de IA está sobrecarregado no momento. Tente novamente em instantes ou preencha manualmente." });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { message = "Erro ao comunicar com o serviço de IA. Preencha manualmente." });
        }
        catch (TaskCanceledException)
        {
            return StatusCode(504, new { message = "O serviço de IA demorou demais para responder. Tente novamente ou preencha manualmente." });
        }
        catch (JsonException)
        {
            return StatusCode(502, new { message = "Não foi possível interpretar a resposta da IA. Preencha manualmente." });
        }
    }
}
