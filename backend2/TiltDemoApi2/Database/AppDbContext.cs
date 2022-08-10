using Microsoft.EntityFrameworkCore;

namespace TiltDemoApi2.Database;

public class AppDbContext : DbContext
{
    public DbSet<Model> SecondData { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}