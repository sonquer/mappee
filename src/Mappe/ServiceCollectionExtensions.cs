using Mappe;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMappe(this IServiceCollection services)
        {
            services.AddSingleton<IMapper, MappeExecutor>();

            return services;
        }
    }
}
