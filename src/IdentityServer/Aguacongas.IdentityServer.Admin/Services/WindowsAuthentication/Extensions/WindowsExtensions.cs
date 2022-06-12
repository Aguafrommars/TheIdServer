using Aguacongas.IdentityServer.Admin.Services.WindowsAuthentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for enabling Windows authentication dynamically.
/// </summary>
public static class WindowsExtensions
{
    /// <summary>
    /// Configures the <see cref="AuthenticationBuilder"/> to use Windows (also known as Negotiate, Kerberos, or NTLM) authentication
    /// using the default scheme. The default scheme is specified by <see cref="NegotiateDefaults.AuthenticationScheme"/>.
    /// <para>
    /// This authentication handler supports Kerberos on Windows and Linux servers.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">Allows for configuring the authentication handler.</param>
    /// <returns>The original builder.</returns>
    public static AuthenticationBuilder AddWindows(this AuthenticationBuilder builder, Action<NegotiateOptions> configureOptions)
        => builder.AddWindows(NegotiateDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Configures the <see cref="AuthenticationBuilder"/> to use Windows (also known as Negotiate, Kerberos, or NTLM) authentication
    /// using the specified authentication scheme.
    /// <para>
    /// This authentication handler supports Kerberos on Windows and Linux servers.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The scheme name used to identify the authentication handler internally.</param>
    /// <param name="configureOptions">Allows for configuring the authentication handler.</param>
    /// <returns>The original builder.</returns>
    public static AuthenticationBuilder AddWindows(this AuthenticationBuilder builder, string authenticationScheme, Action<NegotiateOptions> configureOptions)
        => builder.AddWindows(authenticationScheme, displayName: null, configureOptions: configureOptions);

    /// <summary>
    /// Configures the <see cref="AuthenticationBuilder"/> to use Windows (also known as Negotiate, Kerberos, or NTLM) authentication
    /// using the specified authentication scheme.
    /// <para>
    /// This authentication handler supports Kerberos on Windows and Linux servers.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The scheme name used to identify the authentication handler internally.</param>
    /// <param name="displayName">The name displayed to users when selecting an authentication handler. The default is null to prevent this from displaying.</param>
    /// <param name="configureOptions">Allows for configuring the authentication handler.</param>
    /// <returns>The original builder.</returns>
    public static AuthenticationBuilder AddWindows(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WindowsOptions> configureOptions)
    {
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<WindowsOptions>, PostConfigureWindowsOptions>());
        return builder.AddScheme<WindowsOptions, WindowsHandler>(authenticationScheme, displayName, options =>
        {
            configureOptions(options);
        });
    }
}
