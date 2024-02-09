using System;
using Mappee.Abstraction;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Mappee.Configuration;

public static class ServiceCollectionExtensions
{
    private static readonly Profile ProfileInstance = new();

    public static IServiceCollection AddMappee(this IServiceCollection services, Action<Profile> profile)
    {
        profile.Invoke(ProfileInstance);

        services.AddSingleton<IMapper, MappeExecutor>();

        ProfileInstance.Compile();

        return services;
    }
}