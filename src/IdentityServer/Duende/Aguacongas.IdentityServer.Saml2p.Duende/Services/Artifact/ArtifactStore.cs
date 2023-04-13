using Aguacongas.IdentityServer.Store;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;

/// <summary>
/// Artifact store
/// </summary>
public class ArtifactStore : IArtifactStore
{
    private readonly IAdminStore<Entity.Saml2PArtifact> _store;
    
    /// <summary>
    /// Initialize a new instance of <see cref="ArtifactStore"/>
    /// </summary>
    /// <param name="store"></param>
    public ArtifactStore(IAdminStore<Entity.Saml2PArtifact> store)
    {
        _store = store;
    }

    /// <summary>
    /// Remove a stored artifact from store
    /// </summary>
    /// <param name="artifact"></param>
    /// <returns>The artifact</returns>
    public async Task<Entity.Saml2PArtifact> RemoveAsync(string artifact)
    {
        var entity = await _store.GetAsync(artifact, new GetRequest()).ConfigureAwait(false);
        await _store.DeleteAsync(artifact).ConfigureAwait(false);
        return entity;
    }

    /// <summary>
    /// Stores an artifact
    /// </summary>
    /// <param name="artifact"></param>
    /// <returns></returns>
    public Task StoreAsync(Entity.Saml2PArtifact artifact)
    => _store.CreateAsync(artifact);
}
