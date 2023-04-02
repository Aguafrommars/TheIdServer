using Aguacongas.IdentityServer.Store;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
public class ArtifactStore : IArtifactStore
{
    private readonly IAdminStore<Entity.Saml2pArtifact> _store;
    
    public ArtifactStore(IAdminStore<Entity.Saml2pArtifact> store)
    {
        _store = store;
    }

    public async Task<Entity.Saml2pArtifact> RemoveAsync(string artifact)
    {
        var entity = await _store.GetAsync(artifact, new GetRequest()).ConfigureAwait(false);
        await _store.DeleteAsync(artifact).ConfigureAwait(false);
        return entity;
    }

    public Task StoreAsync(Entity.Saml2pArtifact artifact)
    => _store.CreateAsync(artifact);
}
