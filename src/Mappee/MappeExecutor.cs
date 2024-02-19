using Mappee.Abstraction;

namespace Mappee;

/// <summary>
/// Represents a class that executes mapping operations.
/// </summary>
public class MappeExecutor : IMapper
{
    /// <summary>
    /// Maps an object of type T1 to type T2.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>The mapped object of type T2.</returns>
    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return Mapper.Map<TSource, TDestination>(source);
    }

    /// <summary>
    /// Maps an object of type T1.
    /// </summary>
    /// <typeparam name="T1">The destination type.</typeparam>
    /// <param name="source">The source object.</param>
    /// <returns>The mapped object of type T1.</returns>
    public T1 Map<T1>(object source)
    {
        return Mapper.Map<T1>(source);
    }
}
