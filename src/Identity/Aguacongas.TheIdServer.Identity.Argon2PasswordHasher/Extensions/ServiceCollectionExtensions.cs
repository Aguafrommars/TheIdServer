using Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;
using Geralt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add argon2 password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddArgon2PasswordHasher<TUser>(this IServiceCollection services, Action<Argon2PasswordHasherOptions>? configure = null) where TUser : class
    {
        services.AddOptions<Argon2PasswordHasherOptions>()
            .Configure(options => configure?.Invoke(options))
            .Validate(options => options.Memory > Argon2id.MinMemorySize && options.Interations > Argon2id.MinIterations);

        return services.AddTransient<IArgon2Id, Argon2Id>()
            .AddScoped<IPasswordHasher<TUser>, Argon2PasswordHasher<TUser>>();
    }

    /// <summary>
    /// Add argon2 password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddArgon2PasswordHasher<TUser>(this IServiceCollection services, IConfiguration configuration) where TUser : class
    => services.AddArgon2PasswordHasher<TUser>(configuration.Bind);
}
