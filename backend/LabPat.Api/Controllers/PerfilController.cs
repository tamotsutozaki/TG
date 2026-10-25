using LabPat.Application.Features.Perfil;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

// "Meu perfil" do usuário logado
[ApiController]
[Route("api/perfil")]
[Authorize]
public class PerfilController(IPerfilService perfilService) : ControllerBase
{
    [HttpGet("laboratorio")]
    [Authorize(Roles = "Patologista")]
    public async Task<IActionResult> GetLaboratorio() =>
        Ok(await perfilService.GetLaboratorioAsync());

    [HttpPut("laboratorio")]
    [Authorize(Roles = "Patologista")]
    public async Task<IActionResult> AtualizarLaboratorio([FromBody] AtualizarPerfilLaboratorioInput input) =>
        Ok(await perfilService.AtualizarLaboratorioAsync(input));

    [HttpGet("veterinario")]
    [Authorize(Roles = "Veterinario")]
    public async Task<IActionResult> GetVeterinario() =>
        Ok(await perfilService.GetVeterinarioAsync());

    [HttpPut("veterinario")]
    [Authorize(Roles = "Veterinario")]
    public async Task<IActionResult> AtualizarVeterinario([FromBody] AtualizarPerfilVeterinarioInput input) =>
        Ok(await perfilService.AtualizarVeterinarioAsync(input));

    // Qualquer usuário logado (laboratório, veterinário ou administrador)
    [HttpPut("senha")]
    public async Task<IActionResult> AlterarSenha([FromBody] AlterarSenhaInput input)
    {
        await perfilService.AlterarSenhaAsync(input);
        return NoContent();
    }
}
