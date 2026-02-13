using System;

namespace DSRS.SharedKernel.Extensions;

public static class EnumerableExtensions
{
    public static IReadOnlyList<TDto> MapList<TEntity, TDto>(
            this IEnumerable<TEntity> source,
            Func<TEntity, TDto> mapFunc)
            => [.. source.Select(mapFunc)];
}
