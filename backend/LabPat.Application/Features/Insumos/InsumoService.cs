using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.Insumos;

public class InsumoService(
    IInsumoRepository insumoRepository,
    IUnitOfWork unitOfWork) : IInsumoService
{
    public async Task<IEnumerable<InsumoDto>> GetAllAsync(bool incluirInativos = false)
    {
        var insumos = await insumoRepository.GetAllAsync(incluirInativos);
        return insumos.Select(ToDto);
    }

    public async Task<InsumoDto?> GetByIdAsync(int id)
    {
        var insumo = await insumoRepository.GetByIdAsync(id);
        return insumo is null ? null : ToDto(insumo);
    }

    public async Task<InsumoDto> CreateAsync(CreateInsumoInput input)
    {
        Validacao.Faixa(input.QuantidadeMinima, 0, 999999, "A quantidade mínima");
        var insumo = new Insumo
        {
            Nome = Validacao.Obrigatorio(input.Nome, "o nome do insumo"),
            UnidadeMedida = Validacao.Obrigatorio(input.UnidadeMedida, "a unidade de medida"),
            QuantidadeMinima = input.QuantidadeMinima,
            QuantidadeAtual = 0
        };

        await insumoRepository.AddAsync(insumo);
        await unitOfWork.CommitAsync();

        return ToDto(insumo);
    }

    public async Task<InsumoDto?> UpdateAsync(int id, UpdateInsumoInput input)
    {
        var insumo = await insumoRepository.GetByIdAsync(id);
        if (insumo is null) return null;

        Validacao.Faixa(input.QuantidadeMinima, 0, 999999, "A quantidade mínima");
        insumo.Nome = Validacao.Obrigatorio(input.Nome, "o nome do insumo");
        insumo.UnidadeMedida = Validacao.Obrigatorio(input.UnidadeMedida, "a unidade de medida");
        insumo.QuantidadeMinima = input.QuantidadeMinima;
        insumo.Ativo = input.Ativo;

        insumoRepository.Update(insumo);
        await unitOfWork.CommitAsync();

        return ToDto(insumo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var insumo = await insumoRepository.GetByIdAsync(id);
        if (insumo is null) return false;

        insumo.Ativo = false;
        insumoRepository.Update(insumo);
        await unitOfWork.CommitAsync();

        return true;
    }

    public async Task<InsumoDto?> AjustarQuantidadeAsync(int id, AjustarQuantidadeInput input)
    {
        var insumo = await insumoRepository.GetByIdAsync(id);
        if (insumo is null) return null;

        Validacao.Faixa(input.NovaQuantidade, 0, 999999, "A quantidade");
        insumo.QuantidadeAtual = input.NovaQuantidade;
        insumoRepository.Update(insumo);
        await unitOfWork.CommitAsync();

        return ToDto(insumo);
    }

    private static InsumoDto ToDto(Insumo i) =>
        new(i.Id, i.Nome, i.UnidadeMedida, i.QuantidadeAtual, i.QuantidadeMinima, i.Ativo,
            i.QuantidadeAtual < i.QuantidadeMinima);
}
