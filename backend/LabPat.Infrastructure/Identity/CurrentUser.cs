using LabPat.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LabPat.Infrastructure.Identity;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int Id
    {
        get
        {
            var claim = httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier);
            return int.TryParse(claim?.Value, out var id) ? id : 0;
        }
    }
}
