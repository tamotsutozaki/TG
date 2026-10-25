using LabPat.Application.Features.Vinculos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

// Vínculos do veterinário logado com laboratórios
[ApiController]
[Route("api/vinculos")]
[Authorize(Roles = "Veterinario")]
public class VinculosController(IVinculoService vinculoService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ListarMeus() =>
        Ok(await vinculoService.ListarMeusVinculosAsync());

    [HttpPost]
    public async Task<IActionResult> Solicitar([FromBody] SolicitarVinculoInput input) =>
        Ok(await vinculoService.SolicitarAsync(input.LaboratorioId));
}
