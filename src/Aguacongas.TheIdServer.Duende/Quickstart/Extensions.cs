// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart.Account;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.Duende.Quickstart;

/// <summary>
/// Provides extension methods for IdentityServer authorization requests and MVC controllers.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Determines whether the redirect URI in the <see cref="AuthorizationRequest"/> is for a native client.
    /// </summary>
    /// <param name="context">The <see cref="AuthorizationRequest"/> context.</param>
    /// <returns>
    /// <c>true</c> if the redirect URI does not start with "http" or "https"; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsNativeClient(this AuthorizationRequest context)
    {
        return !context.RedirectUri.StartsWith("https", StringComparison.Ordinal)
           && !context.RedirectUri.StartsWith("http", StringComparison.Ordinal);
    }

    /// <summary>
    /// Returns a loading page view result with a redirect URL.
    /// </summary>
    /// <param name="controller">The MVC <see cref="Controller"/> instance.</param>
    /// <param name="viewName">The name of the view to render.</param>
    /// <param name="redirectUri">The URI to redirect to after loading.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the specified view with a <see cref="RedirectViewModel"/>.
    /// </returns>
    public static IActionResult LoadingPage(this Controller controller, string? viewName, string? redirectUri)
    {
        controller.HttpContext.Response.StatusCode = 200;
        controller.HttpContext.Response.Headers.Location = string.Empty;

        return controller.View(viewName, new RedirectViewModel { RedirectUrl = redirectUri });
    }
}
