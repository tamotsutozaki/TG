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

        if (result is null)
            return Unauthorized(new { message = "E-mail ou senha inválidos." });

        return Ok(result);
    }
}
