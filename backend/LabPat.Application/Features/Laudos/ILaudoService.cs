namespace LabPat.Application.Features.Laudos;

public interface ILaudoService
{
    Task<LaudoDto> CreateAsync(CreateLaudoInput input);
    Task<LaudoDto?> GetByIdAsync(int id);
    Task<LaudoDto?> GetBySolicitacaoAsync(int solicitacaoId);
    Task<byte[]?> GerarPdfAsync(int id);
}
