using LabPat.Domain.Entities;
using LabPat.Domain.Enums;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext context)
    : RepositoryBase<Usuario>(context), IUsuarioRepository
{
    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await Context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

    public async Task<bool> EmailExisteAsync(string email) =>
        await Context.Usuarios.AnyAsync(u => u.Email == email);

    public async Task<IEnumerable<Usuario>> GetVeterinariosAsync() =>
        await Context.Usuarios
            .Include(u => u.VetSolicitante)
            .Where(u => u.Papel == PapelUsuario.Veterinario)
            .OrderBy(u => u.StatusConta == StatusConta.Pendente ? 0 : 1)
            .ThenByDescending(u => u.CriadoEm)
            .ToListAsync();
}
