using LabPat.Application.Features.Vinculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

// Laboratórios na visão do veterinário: busca e acesso pelo link de vínculo
[ApiController]
[Route("api/laboratorios")]
[Authorize(Roles = "Veterinario")]
public class LaboratoriosController(IVinculoService vinculoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Buscar([FromQuery] string? busca) =>
        Ok(await vinculoService.BuscarLaboratoriosAsync(busca));

    // Público: o link pode ser aberto antes de o veterinário entrar ou criar a conta
    [HttpGet("codigo/{codigo}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPorCodigo(string codigo)
    {
        var laboratorio = await vinculoService.GetLaboratorioPorCodigoAsync(codigo);
        return laboratorio is null ? NotFound(new { message = "Laboratório não encontrado." }) : Ok(laboratorio);
    }
}
