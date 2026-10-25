using LabPat.Application.Common;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Vinculos;

public class AcessoVeterinarioService(
    IUsuarioRepository usuarioRepository,
    IVinculoVeterinarioRepository vinculoRepository,
    ILaboratorioRepository laboratorioRepository,
    ICurrentUser currentUser,
    ITenantContext tenantContext) : IAcessoVeterinarioService
{
    public async Task<AcessoVeterinario> ObterAsync()
    {
        var veterinario = await usuarioRepository.GetByIdAsync(currentUser.Id);
        if (veterinario is null || veterinario.Papel != PapelUsuario.Veterinario)
            throw new AcessoNegadoException("Acesso restrito a veterinários.");

        var aprovados = await vinculoRepository.GetLaboratoriosAprovadosAsync(veterinario.Id);
        return new AcessoVeterinario(
            veterinario.Id, veterinario.CrmvNumero ?? string.Empty,
            veterinario.CrmvEstado ?? string.Empty, aprovados);
    }

    public async Task EntrarNoLaboratorioAsync(int laboratorioId)
    {
        var vinculo = await vinculoRepository.GetDoVeterinarioNoLaboratorioAsync(currentUser.Id, laboratorioId);
        if (vinculo?.Status != StatusVinculo.Aprovado)
            throw new AcessoNegadoException("Você não tem vínculo aprovado com este laboratório.");

        var laboratorio = await laboratorioRepository.GetByIdAsync(laboratorioId);
        if (laboratorio is null || !laboratorio.Ativo)
            throw new AcessoNegadoException("Este laboratório não está disponível no momento.");

        tenantContext.Definir(laboratorioId);
    }
}
