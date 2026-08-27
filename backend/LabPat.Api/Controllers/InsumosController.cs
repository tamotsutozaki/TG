using LabPat.Application.Features.Insumos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/insumos")]
[Authorize]
public class InsumosController(IInsumoService insumoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await insumoService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var insumo = await insumoService.GetByIdAsync(id);
        return insumo is null ? NotFound() : Ok(insumo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateInsumoInput input)
    {
        var created = await insumoService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateInsumoInput input)
    {
        var updated = await insumoService.UpdateAsync(id, input);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await insumoService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPatch("{id:int}/quantidade")]
    public async Task<IActionResult> AjustarQuantidade(int id, [FromBody] AjustarQuantidadeInput input)
    {
        var updated = await insumoService.AjustarQuantidadeAsync(id, input);
        return updated is null ? NotFound() : Ok(updated);
    }
}
