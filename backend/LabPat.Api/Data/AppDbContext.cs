using Microsoft.EntityFrameworkCore;

namespace LabPat.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
}
