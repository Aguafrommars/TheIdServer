using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Options;
using System;

namespace Aguacongas.TheIdServer.BlazorApp
{
    internal class OidcOptionsConfiguration : IPostConfigureOptions<RemoteAuthenticationOptions<OidcProviderOptions>>
    {
        private readonly NavigationManager _navigationManager;

        public OidcOptionsConfiguration(NavigationManager navigationManager) => _navigationManager = navigationManager;

        public void Configure(RemoteAuthenticationOptions<OidcProviderOptions> options)
        {
            options.UserOptions.AuthenticationType ??= options.ProviderOptions.ClientId;

            var redirectUri = options.ProviderOptions.RedirectUri;
            if (redirectUri == null || !Uri.TryCreate(redirectUri, UriKind.Absolute, out _))
            {
                redirectUri ??= "authentication/login-callback";
                options.ProviderOptions.RedirectUri = _navigationManager
                    .ToAbsoluteUri(redirectUri).AbsoluteUri;
            }

            var logoutUri = options.ProviderOptions.PostLogoutRedirectUri;
            if (logoutUri == null || !Uri.TryCreate(logoutUri, UriKind.Absolute, out _))
            {
                logoutUri ??= "authentication/logout-callback";
                options.ProviderOptions.PostLogoutRedirectUri = _navigationManager
                    .ToAbsoluteUri(logoutUri).AbsoluteUri;
            }
        }

        public void PostConfigure(string name, RemoteAuthenticationOptions<OidcProviderOptions> options)
        {
            if (string.Equals(name, Options.DefaultName))
            {
                Configure(options);
            }
        }
    }
}
