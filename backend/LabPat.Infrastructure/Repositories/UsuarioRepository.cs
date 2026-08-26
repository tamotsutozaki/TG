using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    public async Task<Usuario?> GetByIdAsync(int id) =>
        await context.Usuarios.FindAsync(id);

    public async Task<IEnumerable<Usuario>> GetAllAsync() =>
        await context.Usuarios.ToListAsync();

    public async Task AddAsync(Usuario entity) =>
        await context.Usuarios.AddAsync(entity);

    public void Update(Usuario entity) =>
        context.Usuarios.Update(entity);

    public void Remove(Usuario entity) =>
        context.Usuarios.Remove(entity);

    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
}
