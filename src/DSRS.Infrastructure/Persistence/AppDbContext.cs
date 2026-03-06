using DSRS.Application.Features.Leaderboards;
using DSRS.Domain.Aggregates.Inventories;
using DSRS.Domain.Aggregates.Items;
using DSRS.Domain.Aggregates.Players;
using DSRS.Domain.Aggregates.Pricing;
using DSRS.Domain.Common;
using DSRS.SharedKernel.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DSRS.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions options, IDateTime dateTimeService) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<DailyPrice> DailyPrices => Set<DailyPrice>();
    public DbSet<Inventory> Inventories => Set<Inventory>();

    // view
    public DbSet<PlayerLeaderboardDto> PlayerLeaderboards { get; set; }

    private readonly IDateTime _dateTimeService = dateTimeService;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<PlayerLeaderboardDto>().HasNoKey().ToView("view-1");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override int SaveChanges() =>
          SaveChangesAsync().GetAwaiter().GetResult();

    public override async Task<int> SaveChangesAsync(CancellationToken ct = new CancellationToken())
    {
        AuditRecord();

        return await base.SaveChangesAsync(ct);
    }

    private void AuditRecord()
    {
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(_dateTimeService.Now);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetModified(_dateTimeService.Now);
                    break;
            }
        }
    }
}
