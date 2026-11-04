// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Admin.Options;

/// <summary>
/// Defines when PKCE is required for clients created by dynamic client registration.
/// </summary>
public enum PkceRequirement
{
    /// <summary>
    /// PKCE is required for every client using the authorization code flow (default, recommended).
    /// </summary>
    Always,

    /// <summary>
    /// PKCE is required only for public clients (clients that do not authenticate at the token endpoint).
    /// Confidential clients can omit it, for example to pass conformance suites that don't send a code challenge.
    /// </summary>
    PublicClientsOnly,

    /// <summary>
    /// PKCE is never required. Not recommended.
    /// </summary>
    Never
}