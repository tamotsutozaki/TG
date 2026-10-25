using LabPat.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace LabPat.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginInput input)
    {
        var result = await authService.LoginAsync(input);

        return result.Status switch
        {
            LoginStatus.Sucesso => Ok(result.Auth),
            LoginStatus.ContaDesativada => StatusCode(403,
                new { message = "Esta conta está desativada. Entre em contato com o administrador do sistema." }),
            _ => Unauthorized(new { message = "E-mail ou senha inválidos." })
        };
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarVetInput input)
    {
        await authService.RegistrarVetAsync(input);
        return Ok(new
        {
            message = "Conta criada! Entre no sistema e peça vínculo com os laboratórios com que você trabalha."
        });
    }

    // POST (e não GET com o token na URL) para o token não ficar registrado em logs de acesso
    [HttpPost("convites/validar")]
    public async Task<IActionResult> ValidarConvite([FromBody] ValidarConviteInput input) =>
        Ok(await authService.ValidarConviteAsync(input.Token));

    [HttpPost("registrar-laboratorio")]
    public async Task<IActionResult> RegistrarLaboratorio([FromBody] RegistrarLaboratorioInput input)
    {
        await authService.RegistrarLaboratorioAsync(input);
        return Ok(new { message = "Laboratório cadastrado! Você já pode entrar com o e-mail e a senha informados." });
    }
}
