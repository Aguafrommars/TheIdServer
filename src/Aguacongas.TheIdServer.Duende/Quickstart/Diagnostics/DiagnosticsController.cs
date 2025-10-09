// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Diagnostics;

/// <summary>
/// Controller for diagnostics operations.
/// Only accessible to authorized users from local addresses.
/// </summary>
[SecurityHeaders]
[Authorize]
public class DiagnosticsController : Controller
{
    /// <summary>
    /// Displays diagnostic information for the authenticated user.
    /// Only accessible from local addresses.
    /// </summary>
    /// <returns>
    /// An <see cref="IActionResult"/> containing the diagnostics view if accessed locally; otherwise, <see cref="NotFoundResult"/>.
    /// </returns>
    public async Task<IActionResult> Index()
    {
        var localAddresses = new string[] { "127.0.0.1", "::1", HttpContext.Connection.LocalIpAddress!.ToString() };
        if (!localAddresses.Contains(HttpContext.Connection.RemoteIpAddress!.ToString()))
        {
            return NotFound();
        }

        var model = new DiagnosticsViewModel(await HttpContext.AuthenticateAsync());
        return View(model);
    }
}