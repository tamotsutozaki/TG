using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Vinculos;

public class VinculoService(
    ILaboratorioRepository laboratorioRepository,
    IVinculoVeterinarioRepository vinculoRepository,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IVinculoService
{
    private const int LimiteBusca = 50;

    public async Task<IEnumerable<LaboratorioBuscaDto>> BuscarLaboratoriosAsync(string? termo)
    {
        var laboratorios = await laboratorioRepository.BuscarAtivosAsync(termo, LimiteBusca);
        var status = await StatusPorLaboratorioAsync();

        return laboratorios.Select(l => new LaboratorioBuscaDto(
            l.Id, l.Nome, l.Cidade, l.Uf, status.GetValueOrDefault(l.Id)));
    }

    public async Task<LaboratorioBuscaDto?> GetLaboratorioPorCodigoAsync(string codigo)
    {
        var laboratorio = await laboratorioRepository.GetByCodigoVinculoAsync(codigo.Trim().ToUpperInvariant());
        if (laboratorio is null || !laboratorio.Ativo) return null;

        // Rota pública: o status do vínculo só aparece para o veterinário logado
        var status = EhVeterinario ? (await StatusPorLaboratorioAsync()).GetValueOrDefault(laboratorio.Id) : null;
        return new LaboratorioBuscaDto(laboratorio.Id, laboratorio.Nome, laboratorio.Cidade, laboratorio.Uf, status);
    }

    public async Task<IEnumerable<MeuVinculoDto>> ListarMeusVinculosAsync()
    {
        var vinculos = await vinculoRepository.GetDoVeterinarioAsync(currentUser.Id);
        return vinculos.Select(ToDto);
    }

    public async Task<MeuVinculoDto> SolicitarAsync(int laboratorioId)
    {
        var laboratorio = await laboratorioRepository.GetByIdAsync(laboratorioId);
        if (laboratorio is null || !laboratorio.Ativo)
            throw new RegraNegocioException("Laboratório não encontrado.");

        var vinculo = await vinculoRepository.GetDoVeterinarioNoLaboratorioAsync(currentUser.Id, laboratorioId);

        if (vinculo is null)
        {
            vinculo = new VinculoVeterinario
            {
                VeterinarioId = currentUser.Id,
                LaboratorioId = laboratorioId
            };
            await vinculoRepository.AddAsync(vinculo);
        }
        else
        {
            switch (vinculo.Status)
            {
                case StatusVinculo.Pendente:
                    throw new RegraNegocioException("Você já pediu vínculo com este laboratório. Aguarde a resposta.");
                case StatusVinculo.Aprovado:
                    throw new RegraNegocioException("Você já tem vínculo com este laboratório.");
            }

            // Recusado ou desfeito: o veterinário pode pedir de novo
            vinculo.Status = StatusVinculo.Pendente;
            vinculo.SolicitadoEm = DateTime.UtcNow;
            vinculo.RespondidoEm = null;
            vinculoRepository.Update(vinculo);
        }

        await unitOfWork.CommitAsync();

        vinculo.Laboratorio = laboratorio;
        return ToDto(vinculo);
    }

    // --- helpers ---

    private bool EhVeterinario => currentUser.Papel == PapelUsuario.Veterinario.ToString();

    private async Task<Dictionary<int, StatusVinculo?>> StatusPorLaboratorioAsync()
    {
        if (!EhVeterinario) return [];

        var vinculos = await vinculoRepository.GetDoVeterinarioAsync(currentUser.Id);
        return vinculos.ToDictionary(v => v.LaboratorioId, v => (StatusVinculo?)v.Status);
    }

    private static MeuVinculoDto ToDto(VinculoVeterinario v) =>
        new(v.Id, v.LaboratorioId, v.Laboratorio.Nome, v.Laboratorio.Cidade, v.Laboratorio.Uf,
            v.Status, v.SolicitadoEm, v.RespondidoEm);
}
