using Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IServiceCollection"/> extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add <see cref="UpgradePasswordHasher{TUser}" /> in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IServiceCollection AddUpgradePasswordHasher<TUser>(this IServiceCollection services, Action<UpgradePasswordHasherOptions>? configure = null) where TUser : class
    {
        services.AddOptions<UpgradePasswordHasherOptions>()
            .Configure(options => configure?.Invoke(options))
            .Validate<IServiceProvider>((options, provider) =>
            {
                var hasherList= provider.GetServices<IPasswordHasher<TUser>>()
                    .Where(h => h.GetType() != typeof(UpgradePasswordHasher<TUser>));
                return options.HashPrefixMaps is not null &&
                    options.HashPrefixMaps.Values.All(m => hasherList.Any(h => h.GetType().FullName!.StartsWith($"{m}`1"))) &&
                    hasherList.Any(h => h.GetType().FullName!.StartsWith($"{options.UsePasswordHasherTypeName}`1"));
            });
                
        return services.AddScoped<IPasswordHasher<TUser>, UpgradePasswordHasher<TUser>>();
    }

    /// <summary>
    /// Add <see cref="UpgradePasswordHasher{TUser}" /> in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddUpgradePasswordHasher<TUser>(this IServiceCollection services, IConfiguration configuration) where TUser : class
    => services.AddUpgradePasswordHasher<TUser>(configuration.Bind);

}
