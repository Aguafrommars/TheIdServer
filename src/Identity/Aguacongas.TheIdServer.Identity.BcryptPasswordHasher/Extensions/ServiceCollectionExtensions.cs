using Aguacongas.TheIdServer.Identity.BcryptPasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add bcrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddBcryptPasswordHasher<TUser>(this IServiceCollection services, Action<BcryptPasswordHasherOptions>? configure = null) where TUser : class
    {
        services.AddOptions<BcryptPasswordHasherOptions>()
            .Configure(options => configure?.Invoke(options))
            .Validate(options => options.WorkFactor > 9);

        return services.AddScoped<IPasswordHasher<TUser>, BcryptPasswordHasher<TUser>>();
    }

    /// <summary>
    /// Add bcrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddBcryptPasswordHasher<TUser>(this IServiceCollection services, IConfiguration configuration) where TUser : class
    => services.AddBcryptPasswordHasher<TUser>(configuration.Bind);
}
