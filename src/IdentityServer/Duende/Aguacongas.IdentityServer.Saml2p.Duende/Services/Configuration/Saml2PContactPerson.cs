using ITfoxtec.Identity.Saml2.Schemas.Metadata;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;
public class Saml2PContactPerson : ContactPerson
{
    public ContactTypes ContactKind 
    {
        get => ContactType;
        set => ContactType = value;
    }
    public Saml2PContactPerson(): base(ContactTypes.Technical)
    {

    }
}
