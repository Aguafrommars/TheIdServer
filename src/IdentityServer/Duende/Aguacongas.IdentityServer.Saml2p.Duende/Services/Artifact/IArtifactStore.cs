using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;

/// <summary>
/// Artifact store interfact
/// </summary>
public interface IArtifactStore
{
    /// <summary>
    /// Removed a stored artifact from store
    /// </summary>
    /// <param name="artifact"></param>
    /// <returns></returns>
    Task<Entity.Saml2PArtifact> RemoveAsync(string artifact);

    /// <summary>
    /// Stores an artifact
    /// </summary>
    /// <param name="artifact"></param>
    /// <returns></returns>
    Task StoreAsync(Entity.Saml2PArtifact artifact);
}
