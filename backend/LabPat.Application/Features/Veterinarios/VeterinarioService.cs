using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Veterinarios;

public class VeterinarioService(
    IVinculoVeterinarioRepository vinculoRepository,
    IUnitOfWork unitOfWork) : IVeterinarioService
{
    public async Task<IEnumerable<VinculoVeterinarioDto>> ListarAsync()
    {
        var vinculos = await vinculoRepository.GetDoLaboratorioAsync();
        return vinculos.Select(ToDto);
    }

    public Task<VinculoVeterinarioDto?> AprovarAsync(int vinculoId) =>
        AlterarStatusAsync(vinculoId, StatusVinculo.Pendente, StatusVinculo.Aprovado,
            "Só é possível aprovar pedidos pendentes.");

    public Task<VinculoVeterinarioDto?> RecusarAsync(int vinculoId) =>
        AlterarStatusAsync(vinculoId, StatusVinculo.Pendente, StatusVinculo.Recusado,
            "Só é possível recusar pedidos pendentes.");

    public Task<VinculoVeterinarioDto?> DesfazerAsync(int vinculoId) =>
        AlterarStatusAsync(vinculoId, StatusVinculo.Aprovado, StatusVinculo.Desfeito,
            "Só é possível desfazer vínculos aprovados.");

    private async Task<VinculoVeterinarioDto?> AlterarStatusAsync(
        int vinculoId, StatusVinculo statusEsperado, StatusVinculo novoStatus, string erro)
    {
        // Consulta filtrada: o laboratório só encontra vínculos dele
        var vinculo = await vinculoRepository.GetByIdComVeterinarioAsync(vinculoId);
        if (vinculo is null) return null;

        if (vinculo.Status != statusEsperado)
            throw new RegraNegocioException(erro);

        vinculo.Status = novoStatus;
        vinculo.RespondidoEm = DateTime.UtcNow;
        vinculoRepository.Update(vinculo);
        await unitOfWork.CommitAsync();

        return ToDto(vinculo);
    }

    private static VinculoVeterinarioDto ToDto(VinculoVeterinario v) =>
        new(v.Id, v.VeterinarioId, v.Veterinario.Nome, v.Veterinario.Email,
            v.Veterinario.CrmvNumero ?? string.Empty, v.Veterinario.CrmvEstado ?? string.Empty,
            v.Veterinario.Telefone, v.Status, v.SolicitadoEm, v.RespondidoEm);
}
