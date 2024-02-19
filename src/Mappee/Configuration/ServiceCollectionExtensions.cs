using Mappee.Abstraction;
using Microsoft.Extensions.DependencyInjection;
using System;

// ReSharper disable once CheckNamespace
namespace Mappee.Configuration;

public static class ServiceCollectionExtensions
{
    private static readonly Profile ProfileInstance = new();

    /// <summary>
    /// Adds Mappee services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="profile">The action to configure the Mappee profile.</param>
    /// <returns>The modified <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddMappee(this IServiceCollection services, Action<Profile> profile)
    {
        profile.Invoke(ProfileInstance);

        services.AddSingleton<IMapper, MappeExecutor>();

        ProfileInstance.Compile();

        return services;
    }
}