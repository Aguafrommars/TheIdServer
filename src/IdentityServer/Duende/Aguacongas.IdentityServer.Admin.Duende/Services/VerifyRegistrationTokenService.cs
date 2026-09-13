using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Duende.Services;

/// <summary>
/// Verifies the registration token and client existence
/// </summary>
/// <param name="store"></param>
internal class VerifyRegistrationTokenService(IAdminStore<Client> store) : IVerifyRegistrationToken
{
    /// <summary>
    /// Gets the current client.
    /// </summary>
    public Client CurrentClient { get; private set; }

    /// <summary>
    /// Verifies if the client exists and sets the current client.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public async Task<bool> ClientExistsAsync(string clientId)
    {
        var client = await store.GetAsync(clientId, new GetRequest()).ConfigureAwait(false);
        if (client != null)
        {
            CurrentClient = client;
        }
        return client != null;
    }

    /// <summary>
    /// Verifies the registration token and sets the current client id.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="currentClientId"></param>
    /// <returns></returns>
    public bool VerifyRegistrationTokenAsync(Guid token, out string currentClientId)
    {
        currentClientId = CurrentClient?.Id;
        return CurrentClient != null && CurrentClient.RegistrationToken == token;
    }
}
