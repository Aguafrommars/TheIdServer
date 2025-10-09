// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Aguacongas.TheIdServer.UI;

/// <summary>
/// Controller responsible for handling external authentication workflows.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ExternalController"/> class.
/// </remarks>
/// <param name="userManager">User manager for application users.</param>
/// <param name="signInManager">Sign-in manager for application users.</param>
/// <param name="interaction">IdentityServer interaction service.</param>
/// <param name="events">Event service for IdentityServer.</param>
/// <param name="logger">Logger instance.</param>
[SecurityHeaders]
[AllowAnonymous]
public class ExternalController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<ExternalController> logger) : Controller
{

    /// <summary>
    /// Initiates a roundtrip to an external authentication provider.
    /// </summary>
    /// <param name="provider">The external authentication provider scheme.</param>
    /// <param name="returnUrl">The URL to return to after authentication.</param>
    /// <returns>An <see cref="IActionResult"/> representing the challenge or redirect.</returns>
    [HttpGet]
    public async Task<IActionResult> Challenge(string provider, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "~/";
        }

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (!Url.IsLocalUrl(returnUrl) && !interaction.IsValidReturnUrl(returnUrl))
        {
            // user might have clicked on a malicious link - should be logged
            throw new InvalidOperationException("invalid return URL");
        }

        var result = await HttpContext.AuthenticateAsync(provider).ConfigureAwait(false);
        if (result.Succeeded)
        {                            
            // windows authentication needs special handling
            return await ProcessWindowsLoginAsync(provider, result.Principal, returnUrl).ConfigureAwait(false);
        }
        else
        {
            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                {
                    { "returnUrl", returnUrl },
                    { "scheme", provider },
                }
            };

            return Challenge(props, provider);
        }
    }

    /// <summary>
    /// Handles post-processing of external authentication.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> representing the result of the authentication callback.</returns>
    [HttpGet]
    public async Task<IActionResult> Callback()
    {
        // read external identity from the temporary cookie
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme).ConfigureAwait(false);
        if (result?.Succeeded != true)
        {
            throw new InvalidOperationException("External authentication error");
        }

        if (logger.IsEnabled(LogLevel.Debug))
        {
            var externalClaims = result.Principal!.Claims.Select(c => $"{c.Type}: {c.Value}");
            logger.LogDebug("External claims: {@claims}", externalClaims);
        }

        // lookup our user and external provider info
        var (user, provider, providerUserId, claims) = await FindUserFromExternalProviderAsync(result).ConfigureAwait(false);
        // this might be where you might initiate a custom workflow for user registration
        // in this sample we don't show how that would be done, as our sample implementation
        // simply auto-provisions new external user
        user ??= await AutoProvisionUserAsync(provider!, providerUserId, claims).ConfigureAwait(false);

        // this allows us to collect any additonal claims or properties
        // for the specific prtotocols used and store them in the local auth cookie.
        // this is typically used to store data needed for signout from those protocols.
        var additionalLocalClaims = new List<Claim>();
        var localSignInProps = new AuthenticationProperties();
        ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);

        // issue authentication cookie for user
        // we must issue the cookie maually, and can't use the SignInManager because
        // it doesn't expose an API to issue additional claims from the login workflow
        var principal = await signInManager.CreateUserPrincipalAsync(user);
        var name = principal.FindFirst(JwtClaimTypes.Name)?.Value ?? user.Id;

        // issue authentication cookie for user
        var isuser = new IdentityServerUser(user.Id)
        {
            DisplayName = user.UserName,
            IdentityProvider = provider,
            AdditionalClaims = additionalLocalClaims
        };
        await HttpContext.SignInAsync(isuser, localSignInProps);

        // delete temporary cookie used during external authentication
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        // retrieve return URL
        var returnUrl = result.Properties!.Items["returnUrl"] ?? "~/";

        // check if external login is in the context of an OIDC request
        var context = await interaction.GetAuthorizationContextAsync(returnUrl);
        await events.RaiseAsync(new UserLoginSuccessEvent(provider, providerUserId, user.Id, name, true, context?.Client.ClientId));

        if (context != null && context.IsNativeClient())
        {
            return this.LoadingPage("Redirect", returnUrl);
        }

        return Redirect(returnUrl);
    }

    /// <summary>
    /// Processes Windows authentication login and issues an external cookie.
    /// </summary>
    /// <param name="provider">The authentication provider scheme.</param>
    /// <param name="user">The authenticated Windows user principal.</param>
    /// <param name="returnUrl">The URL to return to after authentication.</param>
    /// <returns>An <see cref="IActionResult"/> that redirects to the callback.</returns>
    private async Task<IActionResult> ProcessWindowsLoginAsync(string provider, ClaimsPrincipal user, string returnUrl)
    {
        // see if windows auth has already been requested and succeeded
        // we will issue the external cookie and then redirect the
        // user back to the external callback, in essence, treating windows
        // auth the same as any other external authentication mechanism
        var props = new AuthenticationProperties()
        {
            RedirectUri = Url.Action(nameof(Callback)),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", provider },
            }
        };

        var id = new ClaimsIdentity(provider);
        var name = user.Identity!.Name ??
            user.FindFirst(JwtClaimTypes.Name)?.Value ??
            user.FindFirst(ClaimTypes.Name)?.Value ??
            user.FindFirst(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.Name])?.Value ??
            user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
            user.FindFirst(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.NameIdentifier])?.Value;

        id.AddClaim(new Claim(JwtClaimTypes.Subject, name!));
        id.AddClaim(new Claim(JwtClaimTypes.Name, name!));

        await HttpContext.SignInAsync(
            IdentityConstants.ExternalScheme,
            new ClaimsPrincipal(id),
            props).ConfigureAwait(false);

        return Redirect(props.RedirectUri!);
    }

    /// <summary>
    /// Finds a user from the external authentication provider.
    /// </summary>
    /// <param name="result">The authentication result containing the external principal.</param>
    /// <returns>
    /// A tuple containing the user, provider, provider user ID, and claims.
    /// </returns>
    private async Task<(ApplicationUser? user, string? provider, string providerUserId, IEnumerable<Claim> claims)>
        FindUserFromExternalProviderAsync(AuthenticateResult result)
    {
        var externalUser = result.Principal!;

        // try to determine the unique id of the external user (issued by the provider)
        // the most common claim type for that are the sub claim and the NameIdentifier
        // depending on the external provider, some other claim type might be used
        var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                          externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                          externalUser.FindFirst(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[ClaimTypes.NameIdentifier]) ??
                          throw new Exception("Unknown userid");

        // remove the user id claim so we don't include it as an extra claim if/when we provision the user
        var claims = externalUser.Claims.ToList();
        claims.Remove(userIdClaim);

        var provider = result.Properties?.Items["scheme"];
        var providerUserId = userIdClaim.Value;

        // find external user
        var user = await userManager.FindByLoginAsync(provider!, providerUserId);

        return (user, provider, providerUserId, claims);
    }

    /// <summary>
    /// Automatically provisions a new user from external authentication claims.
    /// </summary>
    /// <param name="provider">The external authentication provider scheme.</param>
    /// <param name="providerUserId">The unique user ID from the provider.</param>
    /// <param name="claims">The claims from the external provider.</param>
    /// <returns>The newly provisioned <see cref="ApplicationUser"/>.</returns>
    private async Task<ApplicationUser> AutoProvisionUserAsync(string provider, string providerUserId, IEnumerable<Claim> claims)
    {
        // create a list of claims that we want to transfer into our store
        var filtered = new List<Claim>();

        // user's display name
        var name = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)?.Value ??
            claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        if (name != null)
        {
            filtered.Add(new Claim(JwtClaimTypes.Name, name));
        }
        else
        {
            var first = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.GivenName)?.Value;
            var last = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value ??
                claims.FirstOrDefault(x => x.Type == ClaimTypes.Surname)?.Value;
            if (first != null && last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
            }
            else if (first != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, first));
            }
            else if (last != null)
            {
                filtered.Add(new Claim(JwtClaimTypes.Name, last));
            }
        }

        // email
        var email = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Email)?.Value ??
           claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
        if (email != null)
        {
            filtered.Add(new Claim(JwtClaimTypes.Email, email));
        }

        var user = new ApplicationUser
        {
            UserName = email ?? Guid.NewGuid().ToString(),
        };
        var identityResult = await userManager.CreateAsync(user);
        if (!identityResult.Succeeded) throw new InvalidOperationException(identityResult.Errors.First().Description);

        if (filtered.Count > 0)
        {
            identityResult = await userManager.AddClaimsAsync(user, filtered);
            if (!identityResult.Succeeded) throw new InvalidOperationException(identityResult.Errors.First().Description);
        }

        identityResult = await userManager.AddLoginAsync(user, new UserLoginInfo(provider, providerUserId, provider));
        if (!identityResult.Succeeded) throw new InvalidOperationException(identityResult.Errors.First().Description);

        return user;
    }

    /// <summary>
    /// Processes login callback for OIDC-based external authentication.
    /// Copies session ID and id_token for single sign-out support.
    /// </summary>
    /// <param name="externalResult">The external authentication result.</param>
    /// <param name="localClaims">The local claims to be added.</param>
    /// <param name="localSignInProps">The local sign-in properties.</param>
    private static void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
    {
        // if the external system sent a session id claim, copy it over
        // so we can use it for single sign-out
        var sid = externalResult.Principal!.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
        if (sid != null)
        {
            localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
        }

        // if the external provider issued an id_token, we'll keep it for signout
        var id_token = externalResult.Properties!.GetTokenValue("id_token");
        if (id_token != null)
        {
            localSignInProps.StoreTokens([new AuthenticationToken { Name = "id_token", Value = id_token }]);
        }
    }
}