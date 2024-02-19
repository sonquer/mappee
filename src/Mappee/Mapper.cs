using Mappee.Configuration;
using Mappee.Store;

namespace Mappee;

/// <summary>
/// Provides mapping functionality for objects.
/// </summary>
public static class Mapper
{
    /// <summary>
    /// Binds the source and destination types for mapping.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <returns>The profile for mapping.</returns>
    public static Profile Bind<TSource, TDestination>()
    {
        var profile = new Profile();
        profile.Bind<TSource, TDestination>();
        return profile;
    }

    /// <summary>
    /// Maps the source object to the destination type.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object to map.</param>
    /// <returns>The mapped destination object.</returns>
    public static TDestination Map<TSource, TDestination>(TSource source)
    {
        return InstanceMappingStore.Execute<TDestination>(typeof(TSource), typeof(TDestination), source);
    }

    /// <summary>
    /// Maps the source object to the destination type.
    /// </summary>
    /// <typeparam name="TDestination">The destination type.</typeparam>
    /// <param name="source">The source object to map.</param>
    /// <returns>The mapped destination object.</returns>
    public static TDestination Map<TDestination>(object source)
    {
        return InstanceMappingStore.Execute<TDestination>(source?.GetType(), typeof(TDestination), source);
    }
}
