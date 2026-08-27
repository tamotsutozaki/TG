using LabPat.Application.Features.TiposExame;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/tipos-exame")]
[Authorize]
public class TiposExameController(ITipoExameService tipoExameService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await tipoExameService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var tipo = await tipoExameService.GetByIdAsync(id);
        return tipo is null ? NotFound() : Ok(tipo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTipoExameInput input)
    {
        var created = await tipoExameService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTipoExameInput input)
    {
        var updated = await tipoExameService.UpdateAsync(id, input);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await tipoExameService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/templates")]
    public async Task<IActionResult> AddTemplate(int id, [FromBody] CreateTemplateLaudoInput input)
    {
        var template = await tipoExameService.AddTemplateAsync(id, input);
        return template is null ? NotFound() : Ok(template);
    }
}
