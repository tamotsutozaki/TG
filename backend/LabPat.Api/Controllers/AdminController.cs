using LabPat.Application.Features.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Administrador")]
public class AdminController(IAdminService adminService) : ControllerBase
{
    [HttpGet("convites")]
    public async Task<IActionResult> ListarConvites() =>
        Ok(await adminService.ListarConvitesAsync());

    [HttpPost("convites")]
    public async Task<IActionResult> GerarConvite([FromBody] GerarConviteInput input) =>
        Ok(await adminService.GerarConviteAsync(input));

    [HttpPut("convites/{id:int}/revogar")]
    public async Task<IActionResult> RevogarConvite(int id) =>
        await adminService.RevogarConviteAsync(id) ? NoContent() : NotFound();

    [HttpGet("laboratorios")]
    public async Task<IActionResult> ListarLaboratorios() =>
        Ok(await adminService.ListarLaboratoriosAsync());

    [HttpPut("laboratorios/{id:int}/ativo")]
    public async Task<IActionResult> AlterarLaboratorioAtivo(int id, [FromBody] AlterarAtivoInput input) =>
        await adminService.AlterarLaboratorioAtivoAsync(id, input.Ativo) ? NoContent() : NotFound();

    [HttpGet("veterinarios")]
    public async Task<IActionResult> ListarVeterinarios() =>
        Ok(await adminService.ListarVeterinariosAsync());

    [HttpPut("veterinarios/{id:int}/ativo")]
    public async Task<IActionResult> AlterarVeterinarioAtivo(int id, [FromBody] AlterarAtivoInput input) =>
        await adminService.AlterarVeterinarioAtivoAsync(id, input.Ativo) ? NoContent() : NotFound();
}
