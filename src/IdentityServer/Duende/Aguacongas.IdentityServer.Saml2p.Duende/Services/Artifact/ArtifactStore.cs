using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Services;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
public class ArtifactStore : IArtifactStore
{
    private readonly IAdminStore<Entity.Saml2pArtifact> _store;
    
    public ArtifactStore(IAdminStore<Entity.Saml2pArtifact> store)
    {
        _store = store;
    }

    public Task StoreAsync(Entity.Saml2pArtifact artifact)
    => _store.CreateAsync(artifact);
}
