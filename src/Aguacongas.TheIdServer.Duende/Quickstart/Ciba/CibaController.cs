using Aguacongas.TheIdServer.Duende.Quickstart.Consent;
using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba;

/// <summary>
/// Controller for handling CIBA (Client-Initiated Backchannel Authentication) consent flows.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CibaController"/> class.
/// </remarks>
/// <param name="interaction">The backchannel authentication interaction service.</param>
/// <param name="events">The event service.</param>
/// <param name="localizer">The string localizer.</param>
/// <exception cref="ArgumentNullException"></exception>
[SecurityHeaders]
[Authorize]
public class CibaController(
    IBackchannelAuthenticationInteractionService interaction,
    IEventService events,
    IStringLocalizer<CibaController> localizer) : Controller
{

    /// <summary>
    /// Displays the consent view for a CIBA login request.
    /// </summary>
    /// <param name="id">The internal login request identifier.</param>
    /// <returns>The consent view.</returns>
    [HttpGet]
    public async Task<IActionResult> Consent(string id)
    {
        var request = await interaction.GetLoginRequestByInternalIdAsync(id);
        var model = BuildViewModelAsync(request!, id);

        return View(model);
    }

    /// <summary>
    /// Handles the consent form submission for a CIBA login request.
    /// </summary>
    /// <param name="input">The input model containing user consent data.</param>
    /// <returns>The result of the consent operation.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Consent([FromForm] InputModel input)
    {
        var request = await interaction.GetLoginRequestByInternalIdAsync(input.Id!);
        var viewModel = BuildViewModelAsync(request!, input.Id!, input);

        CompleteBackchannelLoginRequest? result = null;

        // user clicked 'no' - send back the standard 'access_denied' response
        if (input.Button == "no")
        {
            result = new CompleteBackchannelLoginRequest(input.Id!);

            // emit event
            await events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request!.Client.ClientId, request.ValidatedResources.RawScopeValues))
                .ConfigureAwait(false);
        }
        // user clicked 'yes' - validate the data
        else if (input.Button == "yes")
        {
            // if the user consented to some scope, build the response model
            if (input.ScopesConsented != null && input.ScopesConsented.Any())
            {
                var scopes = input.ScopesConsented;
                if (!ConsentOptions.EnableOfflineAccess)
                {
                    scopes = scopes.Where(x => x != StandardScopes.OfflineAccess);
                }

                result = new CompleteBackchannelLoginRequest(input.Id!)
                {
                    ScopesValuesConsented = [.. scopes],
                    Description = input.Description
                };

                // emit event
                await events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request!.Client.ClientId, request.ValidatedResources.RawScopeValues, result.ScopesValuesConsented, false))
                    .ConfigureAwait(false);
            }
            else
            {
                ModelState.AddModelError(string.Empty, ConsentOptions.MustChooseOneErrorMessage);
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, ConsentOptions.InvalidSelectionErrorMessage);
        }

        if (result is not null)
        {
            // communicate outcome of consent back to identityserver
            await interaction.CompleteLoginRequestAsync(result);                
            return RedirectToPage("/Account/Manage/Ciba", new { area = "Identity" });
        }

        return View(viewModel);
    }

    /// <summary>
    /// Builds the view model for the consent view.
    /// </summary>
    /// <param name="request">The backchannel user login request.</param>
    /// <param name="id">The login request identifier.</param>
    /// <param name="model">The input model, if any.</param>
    /// <returns>The consent view model.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    private ViewModel BuildViewModelAsync(BackchannelUserLoginRequest request, string id, InputModel? model = null)
    {
        if (request is null)
        {
            throw new InvalidOperationException(localizer["Invalid login request id."]);
        }
        if (request.Subject.GetSubjectId() == User.GetSubjectId())
        {
            return CreateConsentViewModel(model, id, request);
        }
        throw new InvalidOperationException(localizer["SubjectIds don't match."]);
    }

    /// <summary>
    /// Creates the consent view model.
    /// </summary>
    /// <param name="model">The input model, if any.</param>
    /// <param name="id">The login request identifier.</param>
    /// <param name="request">The backchannel user login request.</param>
    /// <returns>The consent view model.</returns>
    private ViewModel CreateConsentViewModel(InputModel? model, 
        string id,
        BackchannelUserLoginRequest request)
    {
        var vm = new ViewModel
        {
            Input = new InputModel
            {
                Id = id
            },
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            BindingMessage = request.BindingMessage,
            IdentityScopes = [.. request.ValidatedResources
                .Resources
                .IdentityResources
                .Select(x => CreateScopeViewModel(x, model?.ScopesConsented == null || model.ScopesConsented?.Contains(x.Name) == true))]
        };

        var resourceIndicators = request.RequestedResourceIndicators ?? [];
        var apiResources = request.ValidatedResources.Resources.ApiResources.Where(x => resourceIndicators.Contains(x.Name));

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope is not null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
                scopeVm.Resources = [.. apiResources.Where(x => x.Scopes.Contains(parsedScope.ParsedName))
                    .Select(x => new ResourceViewModel
                    {
                        Name = x.Name,
                        DisplayName = x.DisplayName ?? x.Name,
                    })];
                apiScopes.Add(scopeVm);
            }
        }
        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(model is null || model.ScopesConsented?.Contains(StandardScopes.OfflineAccess) == true));
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
    /// <returns>The scope view model.</returns>
    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new ScopeViewModel
        {
            Name = parsedScopeValue.ParsedName,
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
    /// <returns>The scope view model.</returns>
    private static ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    => new()
    {
        Name = identity.Name,
        Value = identity.Name,
        DisplayName = identity.DisplayName ?? identity.Name,
        Description = identity.Description,
        Emphasize = identity.Emphasize,
        Required = identity.Required,
        Checked = check || identity.Required
    };

    /// <summary>
    /// Gets the offline access scope view model.
    /// </summary>
    /// <param name="check">Whether the scope is checked.</param>
    /// <returns>The offline access scope view model.</returns>
    private static ScopeViewModel GetOfflineAccessScope(bool check) 
    => new()
    {
        Value = StandardScopes.OfflineAccess,
        DisplayName = ConsentOptions.OfflineAccessDisplayName,
        Description = ConsentOptions.OfflineAccessDescription,
        Emphasize = true,
        Checked = check
    };
    
}
