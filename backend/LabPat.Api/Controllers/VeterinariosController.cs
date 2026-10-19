using LabPat.Application.Features.Veterinarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/veterinarios")]
[Authorize(Roles = "Patologista")]
public class VeterinariosController(IVeterinarioService veterinarioService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await veterinarioService.ListarAsync());

    [HttpPut("{id:int}/aprovar")]
    public async Task<IActionResult> Aprovar(int id)
    {
        var ok = await veterinarioService.AprovarAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPut("{id:int}/recusar")]
    public async Task<IActionResult> Recusar(int id)
    {
        var ok = await veterinarioService.RecusarAsync(id);
        return ok ? NoContent() : NotFound();
    }
}
