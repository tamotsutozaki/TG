namespace LabPat.Application.Features.Solicitacoes;

public interface ISolicitacaoService
{
    Task<IEnumerable<SolicitacaoDto>> GetAllAsync();
    Task<SolicitacaoDetalhadaDto?> GetByIdAsync(int id);
    Task<SolicitacaoDto> CreateAsync(CreateSolicitacaoInput input);
    Task<SolicitacaoDetalhadaDto?> UpdateStatusAsync(int id, UpdateStatusInput input);
    Task<ConsultaPublicaDto?> GetByCodigoPublicoAsync(string codigo);
}
