using LabPat.Application.Features.Veterinarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

// Pedidos de vínculo e veterinários vinculados ao laboratório logado
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
        var vinculo = await veterinarioService.AprovarAsync(id);
        return vinculo is null ? NotFound() : Ok(vinculo);
    }

    [HttpPut("{id:int}/recusar")]
    public async Task<IActionResult> Recusar(int id)
    {
        var vinculo = await veterinarioService.RecusarAsync(id);
        return vinculo is null ? NotFound() : Ok(vinculo);
    }

    [HttpPut("{id:int}/desfazer")]
    public async Task<IActionResult> Desfazer(int id)
    {
        var vinculo = await veterinarioService.DesfazerAsync(id);
        return vinculo is null ? NotFound() : Ok(vinculo);
    }
}
