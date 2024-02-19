namespace Mappee.Abstraction;

/// <summary>
/// Represents a mapper interface that provides mapping functionality between objects.
/// </summary>
public interface IMapper
{
    /// <summary>
    /// Maps the properties of the source object to the destination object of the specified types.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object to map from.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map<TSource, TDestination>(TSource source);

    /// <summary>
    /// Maps the properties of the source object to the destination object of the specified type.
    /// </summary>
    /// <typeparam name="TDestination">The type of the destination object.</typeparam>
    /// <param name="source">The source object to map from.</param>
    /// <returns>The mapped destination object.</returns>
    TDestination Map<TDestination>(object source);
}
