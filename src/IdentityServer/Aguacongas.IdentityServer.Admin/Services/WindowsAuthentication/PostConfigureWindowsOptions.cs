using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;

/// <summary>
/// Reconfigures the WindowsOptions to defer to the integrated server authentication if present.
/// </summary>
public class PostConfigureWindowsOptions : IPostConfigureOptions<WindowsOptions>
{
    private readonly PostConfigureNegotiateOptions _inner;
    private LdapSettings _settings;

    /// <summary>
    /// Creates a new <see cref="PostConfigureWindowsOptions"/>
    /// </summary>
    /// <param name="serverAuthServices"></param>
    /// <param name="logger"></param>
    public PostConfigureWindowsOptions(IEnumerable<IServerIntegratedAuth> serverAuthServices, ILogger<NegotiateHandler> logger)
    {
        _inner = new PostConfigureNegotiateOptions(serverAuthServices, logger);
    }


    /// <summary>
    /// Invoked to post configure a TOptions instance.
    /// </summary>
    /// <param name="name">The name of the options instance being configured.</param>
    /// <param name="options">The options instance to configure.</param>
    public void PostConfigure(string name, WindowsOptions options)
    {
        var ldapConnection = _settings?.LdapConnection;
        if (ldapConnection != null)
        {
            ldapConnection.Dispose();
        }

        if (options.LdapEnabled)
        {
            options.EnableLdap(settings =>
            {
                settings.ClaimsCacheAbsoluteExpiration = options.ClaimsCacheAbsoluteExpiration;
                settings.ClaimsCacheSize = options.ClaimsCacheSize;
                settings.ClaimsCacheSlidingExpiration = options.ClaimsCacheSlidingExpiration;
                settings.Domain = options.Domain;
                settings.EnableLdapClaimResolution = options.EnableLdapClaimResolution;
                settings.IgnoreNestedGroups = options.IgnoreNestedGroups;
                settings.MachineAccountName = options.MachineAccountName;
                settings.MachineAccountPassword = options.MachineAccountPassword;
                _settings = settings;
            });
        }            

        _inner.PostConfigure(name, options);
    }
}
