using LabPat.Application.Common;
using LabPat.Application.Features.Gemini;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LabPat.Infrastructure.ExternalServices;

public class GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration) : IGeminiService
{
    private const string Prompt = """
        Você é um assistente de laboratório de patologia veterinária.
        Analise este documento (guia de solicitação de exame veterinário) e extraia os dados abaixo.
        Responda APENAS com JSON válido, sem texto adicional.

        Formato esperado:
        {
          "vet_nome": "nome completo do médico veterinário",
          "vet_crmv_numero": "número do CRMV (somente dígitos)",
          "vet_crmv_estado": "estado do CRMV (sigla, ex: SP)",
          "vet_email": "e-mail do veterinário ou null",
          "vet_telefone": "telefone do veterinário ou null",
          "tutor_nome": "nome do proprietário do animal",
          "tutor_telefone": "telefone do tutor",
          "tutor_email": "e-mail do tutor ou null",
          "paciente_nome": "nome do animal",
          "especie": "espécie (ex: Cão, Gato, Ave, Réptil)",
          "raca": "raça do animal ou null",
          "sexo": "Macho, Femea ou NaoInformado",
          "idade": "idade em texto (ex: 3 anos, 6 meses) ou null",
          "peso": "peso como número em kg (ex: 4.5) ou null",
          "tipo_exame": "tipo de exame solicitado ou null",
          "descricao_clinica": "descrição clínica ou motivo do exame ou null"
        }

        Use null para campos não identificados no documento.
        """;

    public async Task<ExtrairSolicitacaoDto> ExtrairDadosAsync(byte[] arquivo, string mimeType)
    {
        var apiKey = configuration["Gemini:ApiKey"]
            ?? throw new InvalidOperationException("Chave da API do Gemini não configurada.");
        var model = configuration["Gemini:Model"] ?? "gemini-1.5-flash";
        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new object[]
                    {
                        new
                        {
                            inline_data = new
                            {
                                mime_type = mimeType,
                                data = Convert.ToBase64String(arquivo)
                            }
                        },
                        new { text = Prompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.1,
                responseMimeType = "application/json"
            }
        };

        var client = httpClientFactory.CreateClient("Gemini");
        var body = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        var response = await client.PostAsync(url, body);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var text = geminiResponse?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text ?? "{}";

        var dados = JsonSerializer.Deserialize<DadosExtraidos>(text,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? new DadosExtraidos();

        return new ExtrairSolicitacaoDto(
            dados.VetNome, dados.VetCrmvNumero, dados.VetCrmvEstado,
            dados.VetEmail, dados.VetTelefone,
            dados.TutorNome, dados.TutorTelefone, dados.TutorEmail,
            dados.PacienteNome, dados.Especie, dados.Raca,
            dados.Sexo, dados.Idade, dados.Peso,
            dados.TipoExame, dados.DescricaoClinica);
    }

    // Modelos internos para deserialização da resposta do Gemini

    private class GeminiResponse
    {
        public List<Candidate>? Candidates { get; set; }
    }

    private class Candidate
    {
        public GeminiContent? Content { get; set; }
    }

    private class GeminiContent
    {
        public List<GeminiPart>? Parts { get; set; }
    }

    private class GeminiPart
    {
        public string? Text { get; set; }
    }

    private class DadosExtraidos
    {
        [JsonPropertyName("vet_nome")] public string? VetNome { get; set; }
        [JsonPropertyName("vet_crmv_numero")] public string? VetCrmvNumero { get; set; }
        [JsonPropertyName("vet_crmv_estado")] public string? VetCrmvEstado { get; set; }
        [JsonPropertyName("vet_email")] public string? VetEmail { get; set; }
        [JsonPropertyName("vet_telefone")] public string? VetTelefone { get; set; }
        [JsonPropertyName("tutor_nome")] public string? TutorNome { get; set; }
        [JsonPropertyName("tutor_telefone")] public string? TutorTelefone { get; set; }
        [JsonPropertyName("tutor_email")] public string? TutorEmail { get; set; }
        [JsonPropertyName("paciente_nome")] public string? PacienteNome { get; set; }
        [JsonPropertyName("especie")] public string? Especie { get; set; }
        [JsonPropertyName("raca")] public string? Raca { get; set; }
        [JsonPropertyName("sexo")] public string? Sexo { get; set; }
        [JsonPropertyName("idade")] public string? Idade { get; set; }
        [JsonPropertyName("peso")] public string? Peso { get; set; }
        [JsonPropertyName("tipo_exame")] public string? TipoExame { get; set; }
        [JsonPropertyName("descricao_clinica")] public string? DescricaoClinica { get; set; }
    }
}
