using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Duende.IdentityServer.Models;
using ITfoxtec.Identity.Saml2;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;
public class SignInValidationResult
{
    public Saml2Request? Saml2Request { get; set; }
    public string? Error { get; set; }
    public string? ErrorMessage { get; set; }
    public Client? Client { get; set; }
    public RelyingParty? RelyingParty { get; set; }
    public bool SignInRequired { get; set; }
    public ClaimsPrincipal? User { get; set; }
}
