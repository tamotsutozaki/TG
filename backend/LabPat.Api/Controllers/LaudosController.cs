using LabPat.Application.Features.Laudos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/laudos")]
[Authorize]
public class LaudosController(ILaudoService laudoService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLaudoInput input)
    {
        try
        {
            var created = await laudoService.CreateAsync(input);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var laudo = await laudoService.GetByIdAsync(id);
        return laudo is null ? NotFound() : Ok(laudo);
    }

    [HttpGet("solicitacao/{solicitacaoId:int}")]
    public async Task<IActionResult> GetBySolicitacao(int solicitacaoId)
    {
        var laudo = await laudoService.GetBySolicitacaoAsync(solicitacaoId);
        return laudo is null ? NotFound() : Ok(laudo);
    }

    [HttpGet("{id:int}/pdf")]
    public async Task<IActionResult> DownloadPdf(int id)
    {
        var pdf = await laudoService.GerarPdfAsync(id);
        if (pdf is null) return NotFound();

        return File(pdf, "application/pdf", $"laudo-{id}.pdf");
    }
}
