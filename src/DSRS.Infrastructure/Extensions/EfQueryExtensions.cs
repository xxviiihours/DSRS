using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;

namespace DSRS.Infrastructure.Extensions;

public static class EfQueryExtensions
{
    public static async Task<IReadOnlyList<TDto>> ProjectToListAsync<TEntity, TDto>(
            this IQueryable<TEntity> query,
            Expression<Func<TEntity, TDto>> selector,
            CancellationToken ct = default)
    {
        return await query.Select(selector).ToListAsync(ct);
    }
}
