// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Models;
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account;

[AllowAnonymous]
[SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Scafolded code")]
public class LoginWith2faModel(SignInManager<ApplicationUser> signInManager,
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<LoginWith2faModel> logger,
    IStringLocalizer<LoginWith2faModel> localizer) : PageModel
{
    private readonly IStringLocalizer _localizer = localizer;

    [BindProperty]
    public InputModel? Input { get; set; }

    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public bool RedirectToReturnUrl { get; set; }

    public class InputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string? TwoFactorCode { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }
    }

    public async Task<IActionResult> OnGetAsync(bool rememberMe, string? returnUrl = null)
    {
        // Ensure the user has gone through the username & password screen first
        var _ = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        ReturnUrl = returnUrl;
        RememberMe = rememberMe;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string userName, bool rememberMe, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        returnUrl = returnUrl ?? Url.Content("~/");

        var user = await signInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new InvalidOperationException($"Unable to load two-factor authentication user.");
        var authenticatorCode = Input!.TwoFactorCode!.Replace(" ", string.Empty).Replace("-", string.Empty);

        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, Input.RememberMachine);

        if (result.Succeeded)
        {
            logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);

            return await OnSiginSuccesss(user, returnUrl);
        }
        else if (result.IsLockedOut)
        {
            logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
            return RedirectToPage("./Lockout");
        }
        else
        {
            logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
            ModelState.AddModelError(string.Empty, _localizer["Invalid authenticator code."]);
            return Page();
        }
    }

    private async Task<IActionResult> OnSiginSuccesss(ApplicationUser user, string returnUrl)
    {
        var context = await interaction.GetAuthorizationContextAsync(returnUrl, default).ConfigureAwait(false);

        await events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName, clientId: context?.Client?.ClientId), default);

        if (context != null)
        {
            if (context.IsNativeClient())
            {
                // if the client is PKCE then we assume it's native, so this change in how to
                // return the response is for better UX for the end user.
                RedirectToReturnUrl = true;
                ReturnUrl = returnUrl;

                return Page();
            }

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            return Redirect(returnUrl);
        }

        // request for a local page
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        else if (string.IsNullOrEmpty(returnUrl))
        {
            return Redirect("~/");
        }

        // user might have clicked on a malicious link - should be logged
        throw new InvalidReturnUrlException();
    }
}
