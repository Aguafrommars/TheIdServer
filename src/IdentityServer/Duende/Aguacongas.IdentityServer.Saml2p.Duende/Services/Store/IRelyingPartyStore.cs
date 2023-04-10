namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;

/// <summary>
/// Relying party store interface
/// </summary>
public interface IRelyingPartyStore
{
    /// <summary>
    /// Finds the relying party by issuer.
    /// </summary>
    /// <param name="issuer">The issuer.</param>
    /// <returns></returns>
    Task<RelyingParty?> FindRelyingPartyAsync(string issuer);
}
