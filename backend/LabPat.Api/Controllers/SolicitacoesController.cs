using LabPat.Application.Features.Solicitacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/solicitacoes")]
[Authorize]
public class SolicitacoesController(ISolicitacaoService solicitacaoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await solicitacaoService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var s = await solicitacaoService.GetByIdAsync(id);
        return s is null ? NotFound() : Ok(s);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSolicitacaoInput input)
    {
        var created = await solicitacaoService.CreateAsync(input);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusInput input)
    {
        var updated = await solicitacaoService.UpdateStatusAsync(id, input);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpGet("consulta/{codigo}")]
    [AllowAnonymous]
    public async Task<IActionResult> ConsultaPublica(string codigo)
    {
        var result = await solicitacaoService.GetByCodigoPublicoAsync(codigo);
        return result is null ? NotFound() : Ok(result);
    }
}
