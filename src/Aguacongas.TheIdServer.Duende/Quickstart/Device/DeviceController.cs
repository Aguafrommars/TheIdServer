// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.Duende.Quickstart.Consent;
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ISConfiguration = Duende.IdentityServer.Configuration;

namespace Aguacongas.IdentityServer.UI.Device;

/// <summary>
/// Controller for handling device authorization flow, including user code capture, confirmation, and consent processing.
/// </summary>
[Authorize]
[SecurityHeaders]
public class DeviceController(
    IDeviceFlowInteractionService interaction,
    IEventService eventService,
    IOptions<ISConfiguration.IdentityServerOptions> options) : Controller
{
    /// <summary>
    /// Displays the initial device authorization page, prompting for user code if not provided.
    /// </summary>
    /// <returns>The user code capture or confirmation view.</returns>
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userCodeParamName = options.Value.UserInteraction.DeviceVerificationUserCodeParameter;
        var userCode = Request.Query[userCodeParamName];
        if (string.IsNullOrWhiteSpace(userCode))
        {
            return View("UserCodeCapture");
        }

        var vm = await BuildViewModelAsync(userCode!);
        if (vm == null) return View("Error");

        vm.ConfirmUserCode = true;
        return View("UserCodeConfirmation", vm);
    }

    /// <summary>
    /// Handles the POST request for user code capture and displays the confirmation view.
    /// </summary>
    /// <param name="userCode">The user code entered by the user.</param>
    /// <returns>The user code confirmation view or error view.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserCodeCapture(string userCode)
    {
        var vm = await BuildViewModelAsync(userCode);
        if (vm == null)
        {
            return View("Error");
        }

        return View("UserCodeConfirmation", vm);
    }

    /// <summary>
    /// Handles the POST callback for device authorization consent.
    /// </summary>
    /// <param name="model">The device authorization input model.</param>
    /// <returns>The result of the consent processing.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Callback(DeviceAuthorizationInputModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return CallbackInternal(model);
    }

    /// <summary>
    /// Internal method to process the device authorization callback.
    /// </summary>
    /// <param name="model">The device authorization input model.</param>
    /// <returns>The result view based on consent processing.</returns>
    private async Task<IActionResult> CallbackInternal(DeviceAuthorizationInputModel model)
    {
        var result = await ProcessConsent(model);
        if (result.HasValidationError)
        {
            return View("Error");
        }

        return View("Success");
    }

    /// <summary>
    /// Processes the user consent for device authorization.
    /// </summary>
    /// <param name="model">The device authorization input model.</param>
    /// <returns>The result of consent processing.</returns>
    private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationInputModel model)
    {
        var result = new ProcessConsentResult();

        var request = await interaction.GetAuthorizationContextAsync(model.UserCode!);
        if (request == null) return result;

        ConsentResponse? grantedConsent = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (model.Button == "no")
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

            // emit event
            await eventService.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
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
                await eventService.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
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
            await interaction.HandleRequestAsync(model.UserCode!, grantedConsent);

            // indicate that's it ok to redirect back to authorization endpoint
            result.RedirectUri = model.ReturnUrl;
            result.Client = request.Client;
        }
        else
        {
            // we need to redisplay the consent UI
            result.ViewModel = await BuildViewModelAsync(model.UserCode!, model);
        }

        return result;
    }

    /// <summary>
    /// Builds the device authorization view model for the given user code and input model.
    /// </summary>
    /// <param name="userCode">The user code for device authorization.</param>
    /// <param name="model">The device authorization input model (optional).</param>
    /// <returns>The device authorization view model, or null if not found.</returns>
    private async Task<DeviceAuthorizationViewModel?> BuildViewModelAsync(string userCode, DeviceAuthorizationInputModel? model = null)
    {
        var request = await interaction.GetAuthorizationContextAsync(userCode);
        if (request != null)
        {
            return CreateConsentViewModel(userCode, model, request);
        }

        return null;
    }

    /// <summary>
    /// Creates the device authorization consent view model from the authorization request.
    /// </summary>
    /// <param name="userCode">The user code for device authorization.</param>
    /// <param name="model">The device authorization input model (optional).</param>
    /// <param name="request">The device flow authorization request.</param>
    /// <returns>The device authorization view model.</returns>
    private DeviceAuthorizationViewModel CreateConsentViewModel(string userCode, DeviceAuthorizationInputModel? model, DeviceFlowAuthorizationRequest request)
    {
        var client = request.Client;
        var vm = new DeviceAuthorizationViewModel
        {
            UserCode = userCode,
            Description = model?.Description,

            RememberConsent = model?.RememberConsent ?? true,
            ScopesConsented = model?.ScopesConsented ?? [],

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
    /// <param name="check">Indicates whether the scope is checked.</param>
    /// <returns>The scope view model.</returns>
    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = apiScope.DisplayName ?? apiScope.Name,
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
    /// <param name="check">Indicates whether the scope is checked.</param>
    /// <returns>The scope view model.</returns>
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
    /// <param name="check">Indicates whether the scope is checked.</param>
    /// <returns>The offline access scope view model.</returns>
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