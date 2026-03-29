using System.Reflection;

namespace DSRS.SharedKernel.Helpers;

/// <summary>
/// Enhanced VogenHelper with support for bidirectional mapping
/// </summary>
public static class VogenHelper
{
    private const string VogenValueObjectAttributeName = "Vogen.ValueObjectAttribute";
    private const string VogenValuePropertyName = "Value";

    /// <summary>
    /// Check if a type is a Vogen value object
    /// </summary>
    public static bool IsVogenValueObject(Type type)
    {
        var hasVogenAttribute = type.GetCustomAttributes()
            .Any(a => a.GetType().FullName == VogenValueObjectAttributeName);

        var hasValueProperty = type.GetProperty(VogenValuePropertyName,
            BindingFlags.Public | BindingFlags.Instance) != null;

        return hasVogenAttribute || hasValueProperty;
    }

    /// <summary>
    /// Get the underlying value from a Vogen value object
    /// </summary>
    public static object? GetUnderlyingValue(object vogenValueObject)
    {
        if (vogenValueObject == null) return null;

        var type = vogenValueObject.GetType();
        var valueProperty = type.GetProperty(VogenValuePropertyName,
            BindingFlags.Public | BindingFlags.Instance);

        return valueProperty?.GetValue(vogenValueObject);
    }

    /// <summary>
    /// Get the underlying value type of a Vogen value object
    /// </summary>
    public static Type? GetUnderlyingValueType(Type vogenType)
    {
        var valueProperty = vogenType.GetProperty(VogenValuePropertyName,
            BindingFlags.Public | BindingFlags.Instance);

        return valueProperty?.PropertyType;
    }

    /// <summary>
    /// Create a Vogen value object from an underlying value using From() method
    /// </summary>
    public static object? CreateVogenInstance(Type vogenType, object? underlyingValue)
    {
        if (underlyingValue == null) return null;

        try
        {
            // Look for From() static method - Vogen generates this
            var fromMethod = vogenType.GetMethod("From",
                BindingFlags.Public | BindingFlags.Static,
                null,
                [GetUnderlyingValueType(vogenType)!],
                null);

            if (fromMethod != null)
            {
                return fromMethod.Invoke(null, [underlyingValue]);
            }

            // Fallback: try constructor
            var constructor = vogenType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance,
                null,
                [GetUnderlyingValueType(vogenType)!],
                null);

            if (constructor != null)
            {
                return constructor.Invoke([underlyingValue]);
            }

            return null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Failed to create Vogen instance of type {vogenType.Name} with value {underlyingValue}",
                ex);
        }
    }

    /// <summary>
    /// Extract underlying value with type conversion
    /// Handles Vogen to Primitive conversion
    /// </summary>
    public static object? ExtractAndConvert(object? value, Type targetType)
    {
        if (value == null)
            return null;

        var valueType = value.GetType();

        // --- Step 1: Handle Vogen value objects ---
        if (IsVogenValueObject(valueType))
        {
            var underlying = GetUnderlyingValue(value);

            if (underlying == null)
                return null;

            // If underlying type matches target, return directly
            if (targetType == underlying.GetType())
                return underlying;

            // Try conversion for primitives / IConvertible
            return SafeConvert(underlying, targetType);
        }

        // --- Step 2: If target is a Vogen type, create an instance ---
        if (IsVogenValueObject(targetType))
        {
            return CreateVogenInstance(targetType, value);
        }

        // --- Step 3: Handle Type objects explicitly ---
        if (value is Type srcType)
        {
            if (targetType == typeof(Type)) return srcType;             
            if (targetType == typeof(string)) return srcType.FullName;
                                            
            return value;
        }

        // --- Step 4: If target type is assignable, return original ---
        if (targetType.IsAssignableFrom(valueType))
            return value;

        // --- Step 5: Attempt standard conversion for primitives ---
        return SafeConvert(value, targetType);
    }

    // Helper method: safe conversion using IConvertible
    private static object? SafeConvert(object value, Type targetType)
    {
        try
        {
            if (value is IConvertible && typeof(IConvertible).IsAssignableFrom(targetType))
                return Convert.ChangeType(value, targetType);

            // If target is string, fallback to ToString
            if (targetType == typeof(string))
                return value.ToString();

            // Cannot convert, return original
            return value;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Cannot convert {value.GetType().Name} to {targetType.Name}", ex);
        }
    }
}