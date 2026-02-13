using System;

namespace DSRS.SharedKernel.Mappings;

public class MapFilterAttribute(string sourceProperty, object matchValue) : Attribute
{
    public string SourceProperty { get; } = sourceProperty;
    public object MatchValue { get; } = matchValue;
}
