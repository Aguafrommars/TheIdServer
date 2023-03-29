using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Artifact;
public interface IArtifactStore
{
    Task<Entity.Saml2pArtifact> RemoveAsync(string artifact);
    Task StoreAsync(Entity.Saml2pArtifact artifact);
}
