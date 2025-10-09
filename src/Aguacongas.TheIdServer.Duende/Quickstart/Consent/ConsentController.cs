// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Consent;

/// <summary>
/// This controller processes the consent UI.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ConsentController"/> class.
/// </remarks>
/// <param name="interaction">The identity server interaction service.</param>
/// <param name="events">The event service.</param>
/// <param name="logger">The logger.</param>
[SecurityHeaders]
[Authorize]
public class ConsentController(
    IIdentityServerInteractionService interaction,
    IEventService events,
    ILogger<ConsentController> logger) : Controller
{
    private readonly IIdentityServerInteractionService _interaction = interaction;
    private readonly IEventService _events = events;
    private readonly ILogger<ConsentController> _logger = logger;

    /// <summary>
    /// Shows the consent screen.
    /// </summary>
    /// <param name="returnUrl">The return URL.</param>
    /// <returns>An <see cref="IActionResult"/> representing the consent view or error view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(string returnUrl)
    {
        var vm = await BuildViewModelAsync(returnUrl);
        if (vm != null)
        {
            return View("Index", vm);
        }

        return View("Error");
    }

    /// <summary>
    /// Handles the consent screen postback.
    /// </summary>
    /// <param name="model">The consent input model.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the consent process.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(ConsentInputModel model)
    {
        var result = await ProcessConsent(model);

        if (result.IsRedirect)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context?.IsNativeClient() == true)
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage("Redirect", result.RedirectUri);
            }

            return Redirect(result.RedirectUri!);
        }

        if (result.HasValidationError)
        {
            ModelState.AddModelError(string.Empty, result.ValidationError!);
        }

        if (result.ShowView)
        {
            return View("Index", result.ViewModel);
        }

        return View("Error");
    }

    /*****************************************/
    /* helper APIs for the ConsentController */
    /*****************************************/

    /// <summary>
    /// Processes the consent input model and returns the result.
    /// </summary>
    /// <param name="model">The consent input model.</param>
    /// <returns>A <see cref="ProcessConsentResult"/> representing the outcome of the consent process.</returns>
    private async Task<ProcessConsentResult> ProcessConsent(ConsentInputModel model)
    {
        var result = new ProcessConsentResult();

        // validate return url is still valid
        var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
        if (request == null) return result;

        ConsentResponse? grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (model.Button == "no")
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

            // emit event
            await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
        }
        // user clicked 'yes' - validate the data
        else if (model.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (model.ScopesConsented != null && model.ScopesConsented.Any())
            {
                var scopes = model.ScopesConsented;
                if (!ConsentOptions.EnableOfflineAccess)
                {
                    scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                }

                grantedConsent = new ConsentResponse
                {
                    RememberConsent = model.RememberConsent,
                    ScopesValuesConsented = [.. scopes],
                    Description = model.Description
                };

                // emit event
                await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
            }
            else
            {
                result.ValidationError = ConsentOptions.MustChooseOneErrorMessage;
            }
        }
        else
        {
            result.ValidationError = ConsentOptions.InvalidSelectionErrorMessage;
        }

        if (grantedConsent != null)
        {
            // communicate outcome of consent back to identityserver
            await _interaction.GrantConsentAsync(request, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            result.RedirectUri = model.ReturnUrl;
            result.Client = request.Client;
        }
        else
        {
            // we need to redisplay the consent UI
            result.ViewModel = await BuildViewModelAsync(model.ReturnUrl, model);
        }

        return result;
    }

    /// <summary>
    /// Builds the consent view model for the specified return URL and input model.
    /// </summary>
    /// <param name="returnUrl">The return URL.</param>
    /// <param name="model">The consent input model (optional).</param>
    /// <returns>A <see cref="ConsentViewModel"/> if the request is valid; otherwise, null.</returns>
    private async Task<ConsentViewModel?> BuildViewModelAsync(string? returnUrl, ConsentInputModel? model = null)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (request != null)
        {
            return CreateConsentViewModel(model, returnUrl, request);
        }
        else
        {
            _logger.LogError("No consent request matching request: {returnUrl}", returnUrl);
        }

        return null;
    }

    /// <summary>
    /// Creates the consent view model from the input model, return URL, and authorization request.
    /// </summary>
    /// <param name="model">The consent input model.</param>
    /// <param name="returnUrl">The return URL.</param>
    /// <param name="request">The authorization request.</param>
    /// <returns>A <see cref="ConsentViewModel"/> representing the consent screen.</returns>
    private ConsentViewModel CreateConsentViewModel(
        ConsentInputModel? model, string? returnUrl,
        AuthorizationRequest request)
    {
        var client = request.Client;
        var vm = new ConsentViewModel
        {
            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? [],
            Description = model?.Description,

            ReturnUrl = returnUrl,

            ClientName = client.ClientName ?? client.ClientId,
            ClientUrl = client.ClientUri,
            ClientLogoUrl = client.LogoUri,
            AllowRememberConsent = client.AllowRememberConsent
        };
        if (client.Properties.TryGetValue("PolicyUrl", out var policyUrl))
        {
            vm.PolicyUrl = policyUrl;
        }
        if (client.Properties.TryGetValue("TosUrl", out var tosUrl))
        {
            vm.TosUrl = tosUrl;
        }            

        vm.IdentityScopes = [.. request.ValidatedResources
                .Resources
                .IdentityResources
                .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null))];

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                apiScopes.Add(scopeVm);
            }
        }
        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
        }
        vm.ApiScopes = apiScopes;

        return vm;
    }

    /// <summary>
    /// Creates a scope view model for an API scope.
    /// </summary>
    /// <param name="parsedScopeValue">The parsed scope value.</param>
    /// <param name="apiScope">The API scope.</param>
    /// <param name="check">Whether the scope is checked.</param>
    /// <returns>A <see cref="ScopeViewModel"/> representing the API scope.</returns>
    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = displayName,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    /// <summary>
    /// Creates a scope view model for an identity resource.
    /// </summary>
    /// <param name="identity">The identity resource.</param>
    /// <param name="check">Whether the scope is checked.</param>
    /// <returns>A <see cref="ScopeViewModel"/> representing the identity resource.</returns>
    private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    /// <summary>
    /// Gets the offline access scope view model.
    /// </summary>
    /// <param name="check">Whether the scope is checked.</param>
    /// <returns>A <see cref="ScopeViewModel"/> representing the offline access scope.</returns>
    private static ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }
}