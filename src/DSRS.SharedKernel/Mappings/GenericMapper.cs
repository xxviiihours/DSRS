using DSRS.SharedKernel.Helpers;
using System;
using System.Collections;
using System.Reflection;

namespace DSRS.SharedKernel.Mappings;


/// <summary>
/// Generic mapper with full bidirectional support:
/// - Primitive to Vogen and vice versa
/// - Collections of both
/// - Nested objects
/// - Complex scenarios
/// </summary>
public static class GenericMapper
{
    /// <summary>
    /// Map source object to destination type (bidirectional)
    /// </summary>
    public static TDest Map<TSource, TDest>(TSource source, object? filterContext = null)
        where TDest : new()
    {
        if (source == null) return default!;

        var dest = new TDest();
        MapProperties(source, dest, filterContext);
        return dest;
    }

    /// <summary>
    /// Map source object to existing destination object
    /// </summary>
    public static void MapTo<TSource>(TSource source, object dest, object? filterContext = null)
    {
        if (source == null || dest == null) return;
        MapProperties(source, dest, filterContext);
    }

    /// <summary>
    /// Map a collection of source objects to destination type collection
    /// </summary>
    public static List<TDest> MapCollection<TSource, TDest>(
        IEnumerable<TSource> sources,
        object? filterContext = null)
        where TDest : new()
    {
        if (sources == null) return [];
        return sources.Select(s => Map<TSource, TDest>(s, filterContext)).ToList();
    }

    private static void MapProperties(object source, object dest, object? filterContext)
    {
        //var srcType = source.GetType();
        //var destType = dest.GetType();

        //var srcProps = srcType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        //var destProps = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //foreach (var dp in destProps)
        //{
        //    if (!dp.CanWrite) continue;

        //    var sp = srcProps.FirstOrDefault(p => p.Name == dp.Name);
        //    if (sp == null) continue;

        //    try
        //    {
        //        var srcValue = sp.GetValue(source);
        //        if (srcValue == null) continue;

        //        var srcValueType = srcValue.GetType();

        //        // CASE 1: Collection mapping (both Vogen and Primitive)
        //        if (typeof(IEnumerable).IsAssignableFrom(dp.PropertyType) &&
        //            dp.PropertyType != typeof(string))
        //        {
        //            MapCollectionBidirectional(srcValue, dp, srcValueType, filterContext);
        //            continue;
        //        }

        //        // CASE 2: Vogen to Any type (bidirectional)
        //        bool srcIsVogen = VogenHelper.IsVogenValueObject(srcValueType);
        //        bool destIsVogen = VogenHelper.IsVogenValueObject(dp.PropertyType);

        //        if (srcIsVogen || destIsVogen)
        //        {
        //            MapVogenBidirectional(srcValue, dest, sp, dp);
        //            continue;
        //        }

        //        // CASE 3: Nested objects (non-Vogen, non-collection)
        //        if (!dp.PropertyType.IsValueType && dp.PropertyType != typeof(string))
        //        {
        //            MapNestedObject(srcValue, dp, filterContext);
        //            continue;
        //        }

        //        // CASE 4: Primitive to Primitive (including value types and string)
        //        dp.SetValue(dest, srcValue);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new InvalidOperationException(
        //            $"Error mapping property {dp.Name} from {srcType.Name} to {destType.Name}",
        //            ex);
        //    }
        //}
        var srcProps = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var destProps = dest.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var dp in destProps)
        {
            if (!dp.CanWrite) continue;

            var sp = srcProps.FirstOrDefault(p => p.Name == dp.Name);
            if (sp == null) continue;

            var srcValue = sp.GetValue(source);
            if (srcValue == null) continue;

            var srcValueType = srcValue.GetType();
            var destType = dp.PropertyType;

            bool srcIsVogen = VogenHelper.IsVogenValueObject(srcValueType);
            bool destIsVogen = VogenHelper.IsVogenValueObject(destType);

            // --- Collections ---
            if (typeof(IEnumerable).IsAssignableFrom(destType) && destType != typeof(string))
            {
                MapCollectionBidirectional(srcValue, dest, dp, filterContext);
                continue;
            }

            // --- Vogen properties ---
            if (srcIsVogen || destIsVogen)
            {
                MapVogenBidirectional(srcValue, dest, dp);
                continue;
            }

            // --- Nested objects (non-Vogen, non-collection) ---
            if (!destType.IsValueType && destType != typeof(string))
            {
                var nestedDest = dp.GetValue(dest) ?? Activator.CreateInstance(destType)!;
                MapProperties(srcValue, nestedDest, filterContext);
                dp.SetValue(dest, nestedDest);
                continue;
            }

            // --- Primitive properties ---
            dp.SetValue(dest, srcValue);
        }
    }

    /// <summary>
    /// Map Vogen value objects in any direction
    /// </summary>
    private static void MapVogenBidirectional(object srcValue, object destObject, PropertyInfo dp)
    {
        var converted = VogenHelper.ExtractAndConvert(srcValue, dp.PropertyType);
        dp.SetValue(destObject, converted);
    }

    /// <summary>
    /// Map collections with Vogen support in both directions
    /// </summary>
    private static void MapCollectionBidirectional(
        object srcValue,
        object destObject,
        PropertyInfo dp,
        object? filterContext)
    {
        var srcEnumerable = (IEnumerable)srcValue;
        var destType = dp.PropertyType;

        // Determine the element type
        var elementType = destType.IsArray
            ? destType.GetElementType()!
            : destType.GetGenericArguments().FirstOrDefault()!;

        var destList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType))!;

        foreach (var item in srcEnumerable)
        {
            object mappedItem;
            if (VogenHelper.IsVogenValueObject(item.GetType()) || VogenHelper.IsVogenValueObject(elementType))
            {
                mappedItem = VogenHelper.ExtractAndConvert(item, elementType)!;
            }
            else if (!elementType.IsValueType && elementType != typeof(string))
            {
                mappedItem = Activator.CreateInstance(elementType)!;
                MapProperties(item, mappedItem, filterContext);
            }
            else
            {
                mappedItem = item;
            }

            destList.Add(mappedItem);
        }

        dp.SetValue(destObject, destList);
    }

    private static void MapNestedObject(object srcValue, PropertyInfo dp, object? filterContext)
    {
        var nestedDest = Activator.CreateInstance(dp.PropertyType)!;
        MapProperties(srcValue, nestedDest, filterContext);
        dp.SetValue(dp.DeclaringType, nestedDest);
    }
}