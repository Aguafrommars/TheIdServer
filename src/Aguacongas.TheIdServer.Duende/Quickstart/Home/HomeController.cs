// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.UI;

[SecurityHeaders]
[AllowAnonymous]
public class HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment) : Controller
{

    /// <summary>
    /// Shows the error page
    /// </summary>
    public async Task<IActionResult> Error(string errorId)
    {
        var vm = new ErrorViewModel();

        // retrieve error details from identityserver
        var message = await interaction.GetErrorContextAsync(errorId, HttpContext.RequestAborted);
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