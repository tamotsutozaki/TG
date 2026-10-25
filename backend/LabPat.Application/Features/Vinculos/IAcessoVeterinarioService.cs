using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Vinculos;

/// <summary>
/// Concentra as regras do que o veterinário logado pode acessar.
/// </summary>
public interface IAcessoVeterinarioService
{
    /// <summary>Dados para filtrar as solicitações visíveis ao veterinário logado.</summary>
    Task<AcessoVeterinario> ObterAsync();

    /// <summary>
    /// Confere se o veterinário tem vínculo aprovado com o laboratório (ativo) e, se tiver,
    /// passa a operar no escopo dele durante a requisição. Caso contrário, lança AcessoNegadoException.
    /// </summary>
    Task EntrarNoLaboratorioAsync(int laboratorioId);
}
