using Mappee.Configuration;
using Mappee.Store;

namespace Mappee;

public static class Mapper
{
    public static Profile Bind<T1, T2>(this Profile profile)
    {
        profile.Map<T1, T2>();
        return profile;
    }

    public static Profile Bind<T1, T2>()
    {
        var profile = new Profile();
        profile.Map<T1, T2>();
        return profile;
    }

    public static T2 Map<T1, T2>(T1 source)
    {
        return InstanceStore.Execute<T2>(typeof(T1), typeof(T2), source);
    }
}