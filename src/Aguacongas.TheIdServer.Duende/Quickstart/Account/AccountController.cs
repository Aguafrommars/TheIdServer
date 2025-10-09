// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart;
using Aguacongas.TheIdServer.Duende.Quickstart.Account;
using Aguacongas.TheIdServer.Models;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;

namespace Aguacongas.TheIdServer.UI;

/// <summary>
/// Controller responsible for handling account-related actions such as login, logout, and access denied.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AccountController"/> class.
/// </remarks>
/// <param name="userManager">The user manager.</param>
/// <param name="signInManager">The sign-in manager.</param>
/// <param name="interaction">The IdentityServer interaction service.</param>
/// <param name="clientStore">The client store.</param>
/// <param name="schemeProvider">The authentication scheme provider.</param>
/// <param name="events">The event service.</param>
/// <param name="urlEncoder">The URL encoder.</param>
/// <param name="localizer">The string localizer.</param>
/// <param name="options">The account options.</param>
[SecurityHeaders]
[AllowAnonymous]
[method: SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Needed")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IIdentityServerInteractionService interaction,
    IClientStore clientStore,
    IAuthenticationSchemeProvider schemeProvider,
    IEventService events,
    UrlEncoder urlEncoder,
    IStringLocalizer<AccountController> localizer,
    IOptions<AccountOptions> options) : Controller
{

    /// <summary>
    /// Provides localized strings for the controller.
    /// </summary>
    private readonly IStringLocalizer _localizer = localizer;

    /// <summary>
    /// Entry point into the login workflow.
    /// </summary>
    /// <param name="returnUrl">The URL to return to after login.</param>
    /// <returns>The login view or external login redirect.</returns>
    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
        // build a model so we know what to show on the login page
        var vm = await BuildLoginViewModelAsync(returnUrl).ConfigureAwait(false);

        if (vm.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            return RedirectToAction("Challenge", "External", new { provider = vm.ExternalLoginScheme, returnUrl });
        }

        return View(vm);
    }

    /// <summary>
    /// Handles postback from username/password login.
    /// </summary>
    /// <param name="model">The login input model.</param>
    /// <param name="button">The button pressed by the user.</param>
    /// <returns>The result of the login attempt.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginInputModel model, string button)
    {
        // check if we are in the context of an authorization request
        var context = await interaction.GetAuthorizationContextAsync(model.ReturnUrl).ConfigureAwait(false);

        // the user clicked the "cancel" button
        if (button != "login")
        {
            return await OnCancel(model, context!).ConfigureAwait(false);
        }

        if (ModelState.IsValid)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberLogin, lockoutOnFailure: true).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return await OnSiginSuccesss(model, context!).ConfigureAwait(false);
            }

            if (result.RequiresTwoFactor)
            {
                return Redirect($"/Identity/Account/LoginWith2fa?rememberMe={model.RememberLogin}&returnUrl={urlEncoder.Encode(model.ReturnUrl!)}");
            }

            await events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId:context?.Client.ClientId)).ConfigureAwait(false);
            ModelState.AddModelError(string.Empty, _localizer.GetString(options.Value.InvalidCredentialsErrorMessage));
        }

        // something went wrong, show form with error
        var vm = await BuildLoginViewModelAsync(model).ConfigureAwait(false);
        return View(vm);
    }

    /// <summary>
    /// Handles successful sign-in and redirects appropriately.
    /// </summary>
    /// <param name="model">The login input model.</param>
    /// <param name="context">The authorization context.</param>
    /// <returns>The redirect result.</returns>
    private async Task<IActionResult> OnSiginSuccesss(LoginInputModel model, AuthorizationRequest context)
    {
        var user = await userManager.FindByNameAsync(model.Username!).ConfigureAwait(false);
        await events.RaiseAsync(new UserLoginSuccessEvent(user!.UserName, user.Id, user.UserName, clientId: context?.Client.ClientId)).ConfigureAwait(false);

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // if the client is PKCE then we assume it's native, so this change in how to
                // return the response is for better UX for the end user.
                return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
            }

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(model.ReturnUrl!);
        }

        // request for a local page
        if (Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }
        else if (string.IsNullOrEmpty(model.ReturnUrl))
        {
            return Redirect("~/");
        }

        // user might have clicked on a malicious link - should be logged
        throw new InvalidReturnUrlException();
    }

    /// <summary>
    /// Handles the cancel action during login.
    /// </summary>
    /// <param name="model">The login input model.</param>
    /// <param name="context">The authorization context.</param>
    /// <returns>The redirect result.</returns>
    private async Task<IActionResult> OnCancel(LoginInputModel model, AuthorizationRequest context)
    {
        if (context != null)
        {
            // if the user cancels, send a result back into IdentityServer as if they 
            // denied the consent (even if this client does not require consent).
            // this will send back an access denied OIDC error response to the client.
            await interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied).ConfigureAwait(false);

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", model.ReturnUrl!);
            }

            return Redirect(model.ReturnUrl!);
        }
        else
        {
            // since we don't have a valid context, then we just go back to the home page
            return Redirect("~/");
        }
    }

    /// <summary>
    /// Shows the logout page.
    /// </summary>
    /// <param name="logoutId">The logout context identifier.</param>
    /// <returns>The logout view or direct logout result.</returns>
    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        // build a model so the logout page knows what to display
        var vm = await BuildLogoutViewModelAsync(logoutId).ConfigureAwait(false);

        if (!vm.ShowLogoutPrompt)
        {
            // if the request for logout was properly authenticated from IdentityServer, then
            // we don't need to show the prompt and can just log the user out directly.
            return await Logout(vm).ConfigureAwait(false);
        }

        return View(vm);
    }

    /// <summary>
    /// Handles logout page postback.
    /// </summary>
    /// <param name="model">The logout input model.</param>
    /// <returns>The logged out view or external sign-out redirect.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(LogoutInputModel model)
    {
        // build a model so the logged out page knows what to display
        var vm = await BuildLoggedOutViewModelAsync(model.LogoutId!).ConfigureAwait(false);

        if (User?.Identity?.IsAuthenticated == true)
        {
            // delete local authentication cookie
            await signInManager.SignOutAsync().ConfigureAwait(false);

            // raise the logout event
            await events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName())).ConfigureAwait(false);
        }

        // check if we need to trigger sign-out at an upstream identity provider
        if (vm.TriggerExternalSignout)
        {
            // build a return URL so the upstream provider will redirect back
            // to us after the user has logged out. this allows us to then
            // complete our single sign-out processing.
            var url = Url.Action("Logout", new { logoutId = vm.LogoutId });

            // this triggers a redirect to the external provider for sign-out
            return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme!);
        }

        return View("LoggedOut", vm);
    }

    /// <summary>
    /// Shows the access denied page.
    /// </summary>
    /// <returns>The access denied view.</returns>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    /*****************************************/
    /* helper APIs for the AccountController */
    /*****************************************/

    /// <summary>
    /// Builds the login view model for the login page.
    /// </summary>
    /// <param name="returnUrl">The return URL after login.</param>
    /// <returns>The login view model.</returns>
    private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
    {
        var context = await interaction.GetAuthorizationContextAsync(returnUrl).ConfigureAwait(false);

        if (context?.IdP != null && await schemeProvider.GetSchemeAsync(context.IdP).ConfigureAwait(false) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            var vm = new LoginViewModel
            {
                EnableLocalLogin = local,
                ReturnUrl = returnUrl,
                Username = context.LoginHint,
            };

            if (!local)
            {
                vm.ExternalProviders = [new ExternalProvider { AuthenticationScheme = context.IdP }];
            }

            return vm;
        }

        var schemes = await schemeProvider.GetAllSchemesAsync().ConfigureAwait(false);
        var settings = options.Value;
        var providers = schemes
            .Where(x => x.DisplayName != null ||
                        x.Name.Equals(settings.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase)
            )
            .Select(x => new ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var allowLocal = true;
        var clientId = context?.Client?.ClientId;
        if (clientId != null)
        {
            var client = await clientStore.FindEnabledClientByIdAsync(clientId).ConfigureAwait(false);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count > 0)
                {
                    providers = [.. providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme!))];
                }
            }
        }

        return new LoginViewModel
        {
            AllowRememberLogin = settings.AllowRememberLogin,
            EnableLocalLogin = allowLocal && settings.AllowLocalLogin,
            ReturnUrl = returnUrl,
            Username = context?.LoginHint,
            ExternalProviders = [.. providers],
            ShowForgotPassworLink = settings.ShowForgotPassworLink,
            ShowRegisterLink = settings.ShowRegisterLink,
            ShowResendEmailConfirmationLink = settings.ShowResendEmailConfirmationLink
        };
    }

    /// <summary>
    /// Builds the login view model from the login input model.
    /// </summary>
    /// <param name="model">The login input model.</param>
    /// <returns>The login view model.</returns>
    private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
    {
        var vm = await BuildLoginViewModelAsync(model.ReturnUrl!).ConfigureAwait(false);
        vm.Username = model.Username;
        vm.RememberLogin = model.RememberLogin;
        return vm;
    }

    /// <summary>
    /// Builds the logout view model for the logout page.
    /// </summary>
    /// <param name="logoutId">The logout context identifier.</param>
    /// <returns>The logout view model.</returns>
    private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
    {
        var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = options.Value.ShowLogoutPrompt };

        if (User?.Identity?.IsAuthenticated != true)
        {
            // if the user is not authenticated, then just show logged out page
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        var context = await interaction.GetLogoutContextAsync(logoutId).ConfigureAwait(false);
        if (context?.ShowSignoutPrompt == false)
        {
            // it's safe to automatically sign-out
            vm.ShowLogoutPrompt = false;
            return vm;
        }

        // show the logout prompt. this prevents attacks where the user
        // is automatically signed out by another malicious web page.
        return vm;
    }

    /// <summary>
    /// Builds the logged out view model for the logged out page.
    /// </summary>
    /// <param name="logoutId">The logout context identifier.</param>
    /// <returns>The logged out view model.</returns>
    private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
    {
        // get context information (client name, post logout redirect URI and iframe for federated signout)
        var logout = await interaction.GetLogoutContextAsync(logoutId).ConfigureAwait(false);

        var vm = new LoggedOutViewModel
        {
            AutomaticRedirectAfterSignOut = options.Value.AutomaticRedirectAfterSignOut,
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
            ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName,
            SignOutIframeUrl = logout?.SignOutIFrameUrl,
            LogoutId = logoutId
        };

        if (User?.Identity?.IsAuthenticated == true)
        {
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                var provider = HttpContext.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                var handler = await provider.GetHandlerAsync(HttpContext, idp); 
                var providerSupportsSignout = handler is IAuthenticationSignOutHandler;

                if (providerSupportsSignout)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    vm.LogoutId ??= await interaction.CreateLogoutContextAsync().ConfigureAwait(false);

                    vm.ExternalAuthenticationScheme = idp;
                }
            }
        }

        return vm;
    }
}