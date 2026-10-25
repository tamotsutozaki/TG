using LabPat.Application.Common;
using LabPat.Domain.Entities;
using LabPat.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LabPat.Infrastructure.Security;

public class JwtTokenGenerator(IConfiguration configuration) : ITokenGenerator
{
    public string Generate(Usuario usuario)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Papel.ToString())
        };

        if (usuario.LaboratorioId is not null)
            claims.Add(new Claim(CurrentUser.LaboratorioClaim, usuario.LaboratorioId.Value.ToString()));

        var expiration = double.Parse(configuration["Jwt:ExpirationHours"] ?? "8");

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiration),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
