using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Aguacongas.TheIdServer.Identity.UpgradePasswordHasher;

/// <summary>
/// Upgrade password hasher
/// </summary>
/// <typeparam name="TUser"></typeparam>
public class UpgradePasswordHasher<TUser> : IPasswordHasher<TUser> where TUser : class
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IOptions<UpgradePasswordHasherOptions> _options;

    /// <summary>
    /// Initialize a new instance of <see cref="UpgradePasswordHasher{TUser}"/>
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    public UpgradePasswordHasher(IServiceProvider serviceProvider, IOptions<UpgradePasswordHasherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);

        _serviceProvider = serviceProvider;
        _options = options;
    }

    /// <inheritdoc/>
    public string HashPassword(TUser user, string password)
    => GetHasher(_options.Value.UsePasswordHasherTypeName)
        .HashPassword(user, password);

    /// <inheritdoc/>
    public PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword, string providedPassword)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        var settings = _options.Value;
        var hash = Convert.FromBase64String(hashedPassword);
        var hashPrefix = hash[0];
        var hasherTypeName = settings.HashPrefixMaps![hashPrefix];
        var hasher = GetHasher(hasherTypeName);

        var result = hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        if (result == PasswordVerificationResult.Failed)
        {
            return result;
        }
        result = hasherTypeName == settings.UsePasswordHasherTypeName ? result : PasswordVerificationResult.SuccessRehashNeeded;
        var deadLineExpired = settings.DeadLineUtc.HasValue && settings.DeadLineUtc.Value < DateTime.UtcNow;

        return deadLineExpired && result == PasswordVerificationResult.SuccessRehashNeeded ? 
            PasswordVerificationResult.Failed : 
            result;
    }

    private IPasswordHasher<TUser> GetHasher(string? hasherTypeName)
    => _serviceProvider.GetServices<IPasswordHasher<TUser>>().First(h => h.GetType().FullName!.StartsWith($"{hasherTypeName}`1"));
}
