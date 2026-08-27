using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;

namespace LabPat.Application.Features.TiposExame;

public class TipoExameService(
    ITipoExameRepository tipoExameRepository,
    IUnitOfWork unitOfWork) : ITipoExameService
{
    public async Task<IEnumerable<TipoExameDto>> GetAllAsync()
    {
        var tipos = await tipoExameRepository.GetAllAtivosAsync();
        return tipos.Select(t => new TipoExameDto(t.Id, t.Nome, t.Descricao, t.PrazoEstimadoDias, t.Ativo));
    }

    public async Task<TipoExameDetalhadoDto?> GetByIdAsync(int id)
    {
        var tipo = await tipoExameRepository.GetByIdComDetalhesAsync(id);
        if (tipo is null) return null;

        var templates = tipo.Templates
            .OrderByDescending(t => t.Versao)
            .Select(t => new TemplateLaudoDto(t.Id, t.Conteudo, t.Versao, t.CriadoEm));

        var insumos = tipo.ExameInsumos
            .Select(ei => new ExameInsumoDto(
                ei.InsumoId, ei.Insumo.Nome, ei.Insumo.UnidadeMedida, ei.QuantidadeConsumida));

        return new TipoExameDetalhadoDto(
            tipo.Id, tipo.Nome, tipo.Descricao, tipo.PrazoEstimadoDias, tipo.Ativo, templates, insumos);
    }

    public async Task<TipoExameDto> CreateAsync(CreateTipoExameInput input)
    {
        var tipo = new TipoExame
        {
            Nome = input.Nome,
            Descricao = input.Descricao,
            PrazoEstimadoDias = input.PrazoEstimadoDias
        };

        await tipoExameRepository.AddAsync(tipo);
        await unitOfWork.CommitAsync();

        return new TipoExameDto(tipo.Id, tipo.Nome, tipo.Descricao, tipo.PrazoEstimadoDias, tipo.Ativo);
    }

    public async Task<TipoExameDto?> UpdateAsync(int id, UpdateTipoExameInput input)
    {
        var tipo = await tipoExameRepository.GetByIdAsync(id);
        if (tipo is null) return null;

        tipo.Nome = input.Nome;
        tipo.Descricao = input.Descricao;
        tipo.PrazoEstimadoDias = input.PrazoEstimadoDias;
        tipo.Ativo = input.Ativo;

        tipoExameRepository.Update(tipo);
        await unitOfWork.CommitAsync();

        return new TipoExameDto(tipo.Id, tipo.Nome, tipo.Descricao, tipo.PrazoEstimadoDias, tipo.Ativo);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tipo = await tipoExameRepository.GetByIdAsync(id);
        if (tipo is null) return false;

        tipo.Ativo = false;
        tipoExameRepository.Update(tipo);
        await unitOfWork.CommitAsync();

        return true;
    }

    public async Task<TemplateLaudoDto?> AddTemplateAsync(int tipoExameId, CreateTemplateLaudoInput input)
    {
        var tipo = await tipoExameRepository.GetByIdComDetalhesAsync(tipoExameId);
        if (tipo is null) return null;

        var ultimaVersao = tipo.Templates.Any() ? tipo.Templates.Max(t => t.Versao) : 0;

        var template = new TemplateLaudo
        {
            TipoExameId = tipoExameId,
            Conteudo = input.Conteudo,
            Versao = ultimaVersao + 1
        };

        tipo.Templates.Add(template);
        await unitOfWork.CommitAsync();

        return new TemplateLaudoDto(template.Id, template.Conteudo, template.Versao, template.CriadoEm);
    }

    public async Task<ExameInsumoDto?> VincularInsumoAsync(int tipoExameId, VincularInsumoInput input)
    {
        var tipo = await tipoExameRepository.GetByIdComDetalhesAsync(tipoExameId);
        if (tipo is null) return null;

        var vinculo = tipo.ExameInsumos.FirstOrDefault(ei => ei.InsumoId == input.InsumoId);

        if (vinculo is not null)
        {
            vinculo.QuantidadeConsumida = input.QuantidadeConsumida;
        }
        else
        {
            vinculo = new ExameInsumo
            {
                TipoExameId = tipoExameId,
                InsumoId = input.InsumoId,
                QuantidadeConsumida = input.QuantidadeConsumida
            };
            tipo.ExameInsumos.Add(vinculo);
        }

        await unitOfWork.CommitAsync();

        var insumo = vinculo.Insumo;
        return new ExameInsumoDto(
            vinculo.InsumoId,
            insumo?.Nome ?? string.Empty,
            insumo?.UnidadeMedida ?? string.Empty,
            vinculo.QuantidadeConsumida);
    }

    public async Task<bool> DesvincularInsumoAsync(int tipoExameId, int insumoId)
    {
        var tipo = await tipoExameRepository.GetByIdComDetalhesAsync(tipoExameId);
        if (tipo is null) return false;

        var vinculo = tipo.ExameInsumos.FirstOrDefault(ei => ei.InsumoId == insumoId);
        if (vinculo is null) return false;

        tipo.ExameInsumos.Remove(vinculo);
        await unitOfWork.CommitAsync();

        return true;
    }
}
