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
            LoginStatus.ContaPendente => StatusCode(403,
                new { message = "Seu cadastro está em análise pelo laboratório." }),
            LoginStatus.ContaRecusada => StatusCode(403,
                new { message = "Seu cadastro não foi aprovado. Entre em contato com o laboratório." }),
            _ => Unauthorized(new { message = "E-mail ou senha inválidos." })
        };
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] RegistrarVetInput input)
    {
        var result = await authService.RegistrarVetAsync(input);

        return result.Status switch
        {
            RegistroStatus.Sucesso => Ok(new
            {
                message = "Cadastro enviado. Seu acesso será liberado após aprovação do laboratório."
            }),
            RegistroStatus.EmailJaCadastrado => Conflict(
                new { message = "Já existe um cadastro com este e-mail." }),
            RegistroStatus.CrmvInvalido => BadRequest(
                new { message = "CRMV inválido. Verifique o número e a UF." }),
            _ => BadRequest(new { message = "Não foi possível concluir o cadastro." })
        };
    }
}
