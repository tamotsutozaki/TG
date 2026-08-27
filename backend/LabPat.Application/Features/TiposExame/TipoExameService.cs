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
        var tipo = await tipoExameRepository.GetByIdComTemplatesAsync(id);
        if (tipo is null) return null;

        var templates = tipo.Templates.Select(t => new TemplateLaudoDto(t.Id, t.Conteudo, t.Versao, t.CriadoEm));
        return new TipoExameDetalhadoDto(tipo.Id, tipo.Nome, tipo.Descricao, tipo.PrazoEstimadoDias, tipo.Ativo, templates);
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
        var tipo = await tipoExameRepository.GetByIdComTemplatesAsync(tipoExameId);
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
}
