using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace LabPat.Infrastructure.Seeding;

public static class DbSeeder
{
    /// <summary>
    /// Cria a conta de administrador do sistema a partir da configuração (seção "Admin"),
    /// caso ainda não exista nenhum administrador. Laboratórios só entram por convite.
    /// </summary>
    public static async Task SeedAsync(AppDbContext context, IConfiguration configuration, ILogger logger)
    {
        if (await context.Usuarios.AnyAsync(u => u.Papel == PapelUsuario.Administrador))
            return;

        var email = configuration["Admin:Email"];
        var senha = configuration["Admin:Senha"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            logger.LogWarning(
                "Nenhum administrador cadastrado. Configure Admin:Email e Admin:Senha para criá-lo.");
            return;
        }

        context.Usuarios.Add(new Usuario
        {
            Nome = configuration["Admin:Nome"] ?? "Administrador",
            Email = email.Trim().ToLowerInvariant(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(senha),
            Ativo = true,
            Papel = PapelUsuario.Administrador
        });

        await context.SaveChangesAsync();
        logger.LogInformation("Administrador {Email} criado.", email);
    }
}
