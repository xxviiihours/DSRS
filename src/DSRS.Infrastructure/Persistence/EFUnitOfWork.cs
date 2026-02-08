using DSRS.Application.Interfaces;
using System.Data.Common;

namespace DSRS.Infrastructure.Persistence;

public class EFUnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await using var transaction =
        await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
