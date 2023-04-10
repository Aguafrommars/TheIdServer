using ITfoxtec.Identity.Saml2.Schemas.Metadata;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Configuration;

/// <summary>
/// Contact person
/// </summary>
public class Saml2PContactPerson : ContactPerson
{
    /// <summary>
    /// Maps <see cref="ContactPerson.ContactType"/>
    /// </summary>
    public ContactTypes ContactKind 
    {
        get => ContactType;
        set => ContactType = value;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="Saml2PContactPerson"/>
    /// </summary>
    public Saml2PContactPerson(): base(ContactTypes.Technical)
    {

    }
}
