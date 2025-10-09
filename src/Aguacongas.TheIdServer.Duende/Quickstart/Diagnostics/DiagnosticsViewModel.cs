// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;
using System.Text;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Diagnostics;

/// <summary>
/// View model for diagnostics information, including authentication result and associated clients.
/// </summary>
public class DiagnosticsViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DiagnosticsViewModel"/> class.
    /// Extracts client information from the authentication result if available.
    /// </summary>
    /// <param name="result">The authentication result containing properties and client list.</param>
    public DiagnosticsViewModel(AuthenticateResult result)
    {
        AuthenticateResult = result;

        if (result.Properties?.Items is null || !result.Properties.Items.TryGetValue("client_list", out var encoded))
        {
            return;
        }

        var bytes = Base64Url.Decode(encoded!);
        var value = Encoding.UTF8.GetString(bytes);

        Clients = JsonConvert.DeserializeObject<string[]>(value);
    }

    /// <summary>
    /// Gets the authentication result.
    /// </summary>
    public AuthenticateResult? AuthenticateResult { get; }

    /// <summary>
    /// Gets the list of client identifiers associated with the authentication result.
    /// </summary>
    public IEnumerable<string>? Clients { get; } = [];
}