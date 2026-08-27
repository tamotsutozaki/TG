using LabPat.Application.Features.Gemini;

namespace LabPat.Application.Common;

public interface IGeminiService
{
    Task<ExtrairSolicitacaoDto> ExtrairDadosAsync(byte[] arquivo, string mimeType);
}
