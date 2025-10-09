// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Home;

/// <summary>
/// Controller responsible for handling home-related actions, including error display.
/// </summary>
/// <remarks>
/// This controller uses <see cref="IIdentityServerInteractionService"/> to retrieve error details
/// and <see cref="IWebHostEnvironment"/> to determine the environment for error description display.
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="HomeController"/> class.
/// </remarks>
/// <param name="interaction">The identity server interaction service.</param>
/// <param name="environment">The web host environment.</param>
[SecurityHeaders]
[AllowAnonymous]
public class HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment) : Controller
{

    /// <summary>
    /// Shows the error page.
    /// </summary>
    /// <param name="errorId">The error identifier.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> that renders the error view with error details.
    /// </returns>
    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        var message = await interaction.GetErrorContextAsync(errorId);
        if (message != null)
        {
            vm.Error = message;

            if (!environment.IsDevelopment())
            {
                // only show in development
                message.ErrorDescription = null;
            }
        }

        return View("Error", vm);
    }
}