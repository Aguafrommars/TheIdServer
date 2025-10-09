// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Account;

/// <summary>
/// Represents the input model for user login.
/// </summary>
public class LoginInputModel
{
    /// <summary>
    /// Gets or sets the username of the user attempting to log in.
    /// </summary>
    [Required]
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password of the user attempting to log in.
    /// </summary>
    [Required]
    public string? Password { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the login should be remembered.
    /// </summary>
    public bool RememberLogin { get; set; }

    /// <summary>
    /// Gets or sets the return URL to redirect to after login.
    /// </summary>
    public string? ReturnUrl { get; set; }
}