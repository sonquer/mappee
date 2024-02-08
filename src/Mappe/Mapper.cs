using Mappe.Configuration;

namespace Mappe
{
    public static class Mapper
    {
        public static Profile<T1, T2> Bind<T1, T2>()
        {
            var profile = new Profile<T1, T2>();
            return profile;
        }

        public static T2 Map<T1, T2>(T1 source)
        {
            return InstanceStore.GetInstance<T1, T2>(typeof(T1), typeof(T2)).Map(source);
        }
    }
}
