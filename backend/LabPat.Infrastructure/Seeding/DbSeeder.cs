using LabPat.Domain.Entities;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Seeding;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Usuarios.AnyAsync())
            return;

        var admin = new Usuario
        {
            Nome = "Patologista",
            Email = "admin@labpat.com",
            SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Ativo = true
        };

        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
    }
}
