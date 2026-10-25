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
        await Context.Usuarios
            .Include(u => u.Laboratorio)
            .FirstOrDefaultAsync(u => u.Email == email);

    public async Task<bool> EmailExisteAsync(string email) =>
        await Context.Usuarios.AnyAsync(u => u.Email == email);

    public async Task<bool> CrmvEmUsoAsync(string numero, string estado) =>
        await Context.Usuarios.AnyAsync(u =>
            u.Papel == PapelUsuario.Veterinario && u.Ativo &&
            u.CrmvNumero == numero && u.CrmvEstado == estado);

    public async Task<bool> ContaAtivaAsync(int id) =>
        await Context.Usuarios.AnyAsync(u =>
            u.Id == id && u.Ativo && (u.Laboratorio == null || u.Laboratorio.Ativo));

    public async Task<IEnumerable<Usuario>> GetVeterinariosAsync() =>
        await Context.Usuarios
            .Where(u => u.Papel == PapelUsuario.Veterinario)
            .OrderBy(u => u.Nome)
            .ToListAsync();
}
