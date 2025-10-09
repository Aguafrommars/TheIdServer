// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer.Models;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Home;

/// <summary>
/// View model for representing error information in the UI.
/// </summary>
public class ErrorViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorViewModel"/> class.
    /// </summary>
    public ErrorViewModel()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ErrorViewModel"/> class with the specified error message.
    /// </summary>
    /// <param name="error">The error message to display.</param>
    public ErrorViewModel(string error)
    {
        Error = new ErrorMessage { Error = error };
    }

    /// <summary>
    /// Gets or sets the <see cref="ErrorMessage"/> containing error details.
    /// </summary>
    public ErrorMessage? Error { get; set; }
}