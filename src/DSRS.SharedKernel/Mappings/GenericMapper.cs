using System;
using System.Collections;
using System.Reflection;

namespace DSRS.SharedKernel.Mappings;


public static class GenericMapper
{
    public static TDest Map<TSource, TDest>(TSource source, object? filterContext = null)
        where TDest : new()
    {
        if (source == null) return default!;

        var dest = new TDest();
        MapProperties(source, dest, filterContext);
        return dest;
    }

    private static void MapProperties(object source, object dest, object? filterContext)
    {
        var srcProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destProps = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var dp in destProps)
        {
            if (!dp.CanWrite) continue;

            var sp = srcProps.FirstOrDefault(p => p.Name == dp.Name);
            if (sp == null) continue;

            var srcValue = sp.GetValue(source);
            if (srcValue == null) continue;

            // Check for MapFilterAttribute
            var filterAttr = dp.GetCustomAttribute<MapFilterAttribute>();
            if (filterAttr != null && srcValue is IEnumerable srcEnumerable)
            {
                var elementType = dp.PropertyType.GetGenericArguments().First();
                var listType = typeof(List<>).MakeGenericType(elementType);
                var list = (IList)Activator.CreateInstance(listType)!;

                foreach (var item in srcEnumerable)
                {
                    var propInfo = item.GetType().GetProperty(filterAttr.SourceProperty);
                    if (propInfo == null) continue;

                    var val = propInfo.GetValue(item);
                    var match = filterAttr.MatchValue switch
                    {
                        "Today" => val is DateOnly d && d == DateOnly.FromDateTime(DateTime.Now),
                        _ => Equals(val, filterAttr.MatchValue)
                    };

                    if (!match) continue;

                    var mappedItem = Activator.CreateInstance(elementType)!;
                    MapProperties(item, mappedItem, filterContext);
                    list.Add(mappedItem);
                }

                dp.SetValue(dest, list);
                continue;
            }

            // Collections
            if (typeof(IEnumerable).IsAssignableFrom(dp.PropertyType) && dp.PropertyType != typeof(string))
            {
                var elementType = dp.PropertyType.GetGenericArguments().First();
                if (srcValue is IEnumerable srcEnumerable2)
                {
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    var list = (IList)Activator.CreateInstance(listType)!;

                    foreach (var item in srcEnumerable2)
                    {
                        var mappedItem = Activator.CreateInstance(elementType)!;
                        MapProperties(item, mappedItem, filterContext);
                        list.Add(mappedItem);
                    }

                    dp.SetValue(dest, list);
                }
            }
            // Nested object
            else if (!dp.PropertyType.IsValueType && dp.PropertyType != typeof(string))
            {
                var nestedDest = Activator.CreateInstance(dp.PropertyType)!;
                MapProperties(srcValue, nestedDest, filterContext);
                dp.SetValue(dest, nestedDest);
            }
            else
            {
                // Value type or primitive
                dp.SetValue(dest, srcValue);
            }
        }
    }
}

