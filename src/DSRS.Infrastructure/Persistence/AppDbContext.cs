using DSRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DSRS.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<DailyPrice> DailyPrices => Set<DailyPrice>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();
}
