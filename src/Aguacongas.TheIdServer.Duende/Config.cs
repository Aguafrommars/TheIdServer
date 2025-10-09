// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer;

/// <summary>
/// Provides configuration methods for IdentityServer resources, clients, APIs, and relying parties.
/// </summary>
public static class Config
{
    /// <summary>
    /// Gets the seed page routes.
    /// </summary>
    internal static string[] SeedPage { get; } = ["/seed"];

    /// <summary>
    /// Gets the default identity resources for IdentityServer.
    /// </summary>
    /// <returns>
    /// An <see cref="IEnumerable{IdentityResource}"/> containing the identity resources.
    /// </returns>
    public static IEnumerable<IdentityResource> GetIdentityResources()
    {
        var profile = new IdentityResources.Profile();
        profile.UserClaims.Add("role");
        return
        [
            profile,
            new IdentityResources.OpenId(),
            new IdentityResources.Address(),
            new IdentityResources.Email(),
            new IdentityResources.Phone(),
        ];
    }

    /// <summary>
    /// Gets the API resources from configuration, hashing shared secrets.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{ApiResource}"/> containing the API resources.
    /// </returns>
    public static IEnumerable<ApiResource> GetApis(IConfiguration configuration)
    {
        var apiList = configuration.GetSection("InitialData:Apis").Get<IEnumerable<ApiResource>>() ?? Array.Empty<ApiResource>();
        foreach (var api in apiList)
        {
            foreach (var secret in api.ApiSecrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret))
            {
                secret.Value = HashExtensions.Sha256(secret.Value);
            }
            yield return api;
        }
    }

    /// <summary>
    /// Gets the API scopes from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{ApiScope}"/> containing the API scopes.
    /// </returns>
    public static IEnumerable<ApiScope> GetApiScopes(IConfiguration configuration)
        => configuration.GetSection("InitialData:ApiScopes").Get<IEnumerable<ApiScope>>() ?? Array.Empty<ApiScope>();

    /// <summary>
    /// Gets the clients from configuration, hashing shared secrets.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Client}"/> containing the clients.
    /// </returns>
    public static IEnumerable<Client> GetClients(IConfiguration configuration)
    {
        var clientList = configuration.GetSection("InitialData:Clients").Get<IEnumerable<Client>>() ?? Array.Empty<Client>();
        foreach (var client in clientList)
        {
            foreach (var secret in client.ClientSecrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret))
            {
                secret.Value = HashExtensions.Sha256(secret.Value);
            }
            yield return client;
        }
    }

    /// <summary>
    /// Gets the relying parties from configuration.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>
    /// An <see cref="IEnumerable{Entity.RelyingParty}"/> containing the relying parties.
    /// </returns>
    public static IEnumerable<Entity.RelyingParty> GetRelyingParties(IConfiguration configuration)
        => configuration.GetSection("InitialData:RelyingParties").Get<IEnumerable<Entity.RelyingParty>>() ?? Array.Empty<Entity.RelyingParty>();
}