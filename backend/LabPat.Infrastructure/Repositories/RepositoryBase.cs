using LabPat.Domain.Interfaces;
using LabPat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Repositories;

public abstract class RepositoryBase<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;

    public async Task<T?> GetByIdAsync(int id) =>
        await Context.Set<T>().FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await Context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity) =>
        await Context.Set<T>().AddAsync(entity);

    public void Update(T entity) =>
        Context.Set<T>().Update(entity);

    public void Remove(T entity) =>
        Context.Set<T>().Remove(entity);
}
