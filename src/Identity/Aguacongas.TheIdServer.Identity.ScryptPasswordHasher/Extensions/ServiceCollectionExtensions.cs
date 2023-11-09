using Aguacongas.TheIdServer.Identity.ScryptPasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Scrypt;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add Scrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddScryptPasswordHasher<TUser>(this IServiceCollection services, Action<ScryptPasswordHasherOptions>? configure = null) where TUser : class
    {
        services.AddOptions<ScryptPasswordHasherOptions>()
            .Configure(options => configure?.Invoke(options))
            .Validate(options =>
            {
                var N = options.IterationCount;
                var r = options.BlockSize;
                var p = options.ThreadCount;

                return N > 1 &&
                (N & (N - 1)) != 0 &&
                r > 0 &&
                p > 0 &&
                !((ulong)r * (ulong)p >= 1 << 30 ||
                    r > int.MaxValue / 128 / p || 
                    r > int.MaxValue / 256 || 
                    N > int.MaxValue / 128 / r);
            });

        return services.AddTransient(p =>
            {
                var options = p.GetRequiredService<IOptions<ScryptPasswordHasherOptions>>().Value;
                return new ScryptEncoder(options.IterationCount, options.BlockSize, options.ThreadCount);
            })
            .AddScoped<IPasswordHasher<TUser>, ScryptPasswordHasher<TUser>>();
    }

    /// <summary>
    /// Add Scrypt password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddScryptPasswordHasher<TUser>(this IServiceCollection services, IConfiguration configuration) where TUser : class
    => services.AddScryptPasswordHasher<TUser>(configuration.Bind);
}
