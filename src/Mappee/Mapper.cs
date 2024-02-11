using Mappee.Configuration;
using Mappee.Store;

namespace Mappee;

public static class Mapper
{
    public static Profile Bind<TSource, TDestination>(this Profile profile)
    {
        profile.Map<TSource, TDestination>();
        return profile;
    }

    public static Profile Bind<TSource, TDestination>()
    {
        var profile = new Profile();
        profile.Map<TSource, TDestination>();
        return profile;
    }

    public static TDestination Map<TSource, TDestination>(TSource source)
    {
        return InstanceStore.Execute<TDestination>(typeof(TSource), typeof(TDestination), source);
    }

    public static TDestination Map<TDestination>(object source)
    {
        return InstanceStore.Execute<TDestination>(source?.GetType(), typeof(TDestination), source);
    }
}