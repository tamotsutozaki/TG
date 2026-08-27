using LabPat.Domain.Entities;
using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public class UsuarioRepository(AppDbContext context)
    : RepositoryBase<Usuario>(context), IUsuarioRepository
{
    public async Task<Usuario?> GetByEmailAsync(string email) =>
        await Context.Usuarios.FirstOrDefaultAsync(u => u.Email == email && u.Ativo);
}
