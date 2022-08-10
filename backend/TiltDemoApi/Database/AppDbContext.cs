using Microsoft.EntityFrameworkCore;

namespace TiltDemoApi.Database;

public class AppDbContext : DbContext
{
    public DbSet<Model> FirstData { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}