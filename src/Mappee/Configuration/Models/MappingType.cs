namespace Mappee.Configuration.Models;

/// <summary>
/// Represents the type of mapping used in the Mappee configuration.
/// </summary>
internal enum MappingType
{
    /// <summary>
    /// Indicates a plain mapping where the source and destination properties have the same name.
    /// </summary>
    Plain,

    /// <summary>
    /// Indicates a mapping where the source property is mapped to a destination property with a different name.
    /// </summary>
    PropertyToProperty,

    /// <summary>
    /// Indicates a mapping where the source property is mapped to a constant value in the destination.
    /// </summary>
    PropertyToConst,

    /// <summary>
    /// Indicates that the mapping should be ignored and not performed.
    /// </summary>
    IgnoreMapping
}
