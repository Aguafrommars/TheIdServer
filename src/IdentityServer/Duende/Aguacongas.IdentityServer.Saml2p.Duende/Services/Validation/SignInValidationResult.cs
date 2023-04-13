using Aguacongas.IdentityServer.Saml2p.Duende.Services.Store;
using Duende.IdentityServer.Models;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Http;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.Saml2p.Duende.Services.Validation;

/// <summary>
/// Signing validation result
/// </summary>
/// <typeparam name="T"></typeparam>
public class SignInValidationResult<T> where T : Saml2Binding<T>
{
    /// <summary>
    /// Saml2P request
    /// </summary>
    public Saml2Request? Saml2Request { get; set; }

    /// <summary>
    /// Error
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Client
    /// </summary>
    public Client? Client { get; set; }

    /// <summary>
    /// Relying party
    /// </summary>
    public RelyingParty RelyingParty { get; set; } = new RelyingParty();

    /// <summary>
    /// Is sign in required
    /// </summary>
    public bool SignInRequired { get; set; }

    /// <summary>
    /// User
    /// </summary>
    public ClaimsPrincipal? User { get; set; }

    /// <summary>
    /// Saml2 binding
    /// </summary>
    public T? Saml2Binding { get; set; }

    /// <summary>
    /// Generic request
    /// </summary>
    public HttpRequest? GerericRequest { get;  set; }
}
