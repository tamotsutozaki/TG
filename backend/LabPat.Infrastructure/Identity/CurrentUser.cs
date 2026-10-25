using LabPat.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LabPat.Infrastructure.Identity;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public const string LaboratorioClaim = "laboratorioId";

    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public int Id
    {
        get
        {
            var claim = User?.FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }
    }

    public string? Papel => User?.FindFirst(ClaimTypes.Role)?.Value;

    public int? LaboratorioId
    {
        get
        {
            var claim = User?.FindFirst(LaboratorioClaim);
            return int.TryParse(claim?.Value, out var id) ? id : null;
        }
    }
}
