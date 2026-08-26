using Microsoft.EntityFrameworkCore;

namespace LabPat.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
