using System;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Duende.Services;

/// <summary>
/// Interface to verify registration token and client existence
/// </summary>
internal interface IVerifyRegistrationToken
{
    /// <summary>
    /// Verifies the registration token asynchronous.
    /// </summary>
    /// <param name="token"></param>
    /// <param name="currentClientId"></param>
    /// <returns></returns>
    bool VerifyRegistrationTokenAsync(Guid token, out string currentClientId);

    /// <summary>
    /// Verifies if a client exists asynchronous.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns></returns>
    Task<bool> ClientExistsAsync(string clientId);
}
