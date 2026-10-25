using LabPat.Application.Common;
using LabPat.Domain.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Data;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    // Violação de índice único no SQL Server
    private static readonly int[] ErrosChaveDuplicada = [2601, 2627];

    public async Task<int> CommitAsync()
    {
        try
        {
            return await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflitoException("Os dados foram alterados por outra operação. Tente novamente.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sql
                                           && ErrosChaveDuplicada.Contains(sql.Number))
        {
            throw new ConflitoException("Já existe um registro com estes dados.");
        }
    }

    public void Dispose() => context.Dispose();
}
