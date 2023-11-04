using Aguacongas.TheIdServer.UI;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.TheIdServer.Duende.Quickstart.Ciba
{
    [SecurityHeaders]
    [Authorize]
    public class CibaController : Controller
    {
        private readonly IBackchannelAuthenticationInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IStringLocalizer<CibaController> _localizer;
        public CibaController(IBackchannelAuthenticationInteractionService interaction,
            IEventService events,
            IStringLocalizer<CibaController> localizer)
        {
            _interaction = interaction ?? throw new ArgumentNullException(nameof(interaction));
            _events = events ?? throw new ArgumentNullException(nameof(events));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        [HttpGet]
        public async Task<IActionResult> Consent(string id)
        {
            var request = await _interaction.GetLoginRequestByInternalIdAsync(id);
            var model = BuildViewModelAsync(request!, id);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consent([FromForm] InputModel input)
        {
            var request = await _interaction.GetLoginRequestByInternalIdAsync(input.Id!);
            var viewModel = BuildViewModelAsync(request!, input.Id!, input);

            CompleteBackchannelLoginRequest? result = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (input.Button == "no")
            {
                result = new CompleteBackchannelLoginRequest(input.Id!);

                // emit event
                await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request!.Client.ClientId, request.ValidatedResources.RawScopeValues))
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
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = input.Description
                    };

                    // emit event
                    await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request!.Client.ClientId, request.ValidatedResources.RawScopeValues, result.ScopesValuesConsented, false))
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
                await _interaction.CompleteLoginRequestAsync(result);                
                return RedirectToPage("/Account/Manage/Ciba", new { area = "Identity" });
            }

            return View(viewModel);
        }

        private ViewModel BuildViewModelAsync(BackchannelUserLoginRequest request, string id, InputModel? model = null)
        {
            if (request is null)
            {
                throw new InvalidOperationException(_localizer["Invalid login request id."]);
            }
            if (request.Subject.GetSubjectId() == User.GetSubjectId())
            {
                return CreateConsentViewModel(model, id, request);
            }
            throw new InvalidOperationException(_localizer["SubjectIds don't match."]);
        }

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
                BindingMessage = request.BindingMessage
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources
                .Select(x => CreateScopeViewModel(x, model?.ScopesConsented == null || model.ScopesConsented?.Contains(x.Name) == true))
                .ToArray();

            var resourceIndicators = request.RequestedResourceIndicators ?? Enumerable.Empty<string>();
            var apiResources = request.ValidatedResources.Resources.ApiResources.Where(x => resourceIndicators.Contains(x.Name));

            var apiScopes = new List<ScopeViewModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope is not null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope, model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
                    scopeVm.Resources = apiResources.Where(x => x.Scopes.Contains(parsedScope.ParsedName))
                        .Select(x => new ResourceViewModel
                        {
                            Name = x.Name,
                            DisplayName = x.DisplayName ?? x.Name,
                        }).ToArray();
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
}
