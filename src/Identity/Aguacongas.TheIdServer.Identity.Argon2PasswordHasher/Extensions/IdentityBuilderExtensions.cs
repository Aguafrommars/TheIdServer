using Aguacongas.TheIdServer.Identity.Argon2PasswordHasher;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// <see cref="IdentityBuilder"/> extensions
/// </summary>
public static class IdentityBuilderExtensions
{
    /// <summary>
    /// Add argon2 password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configure"></param>
    /// <returns></returns>
    public static IdentityBuilder AddArgon2PasswordHasher<TUser>(this IdentityBuilder builder, Action<Argon2PasswordHasherOptions>? configure = null) where TUser : class
    {
        builder.Services.AddArgon2PasswordHasher<TUser>(configure);
        return builder;
    }

    /// <summary>
    /// Add argon2 password hasher services in DI
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IdentityBuilder AddArgon2PasswordHasher<TUser>(this IdentityBuilder builder, IConfiguration configuration) where TUser : class
    {
        builder.Services.AddArgon2PasswordHasher<TUser>(configuration);
        return builder;
    }
}
