// Project: Aguafrommars/TheIdServer
// Copyright (c) 2026 @Olivier Lefebvre
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account.Manage;

public class GrantsModel(IIdentityServerInteractionService interaction,
    IClientStore clients,
    IResourceStore resources,
    IEventService events) : PageModel
{
    private readonly IResourceStore _resources = resources;

    public IEnumerable<GrantViewModel>? Grants { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        await BuildViewModelAsync().ConfigureAwait(false);
        return Page();
    }

    public async Task<IActionResult> OnPostRevokeAsync(string clientId)
    {
        await interaction.RevokeUserConsentAsync(clientId.Normalize(), HttpContext.RequestAborted);
        await events.RaiseAsync(new GrantsRevokedEvent(User.GetSubjectId(), clientId), HttpContext.RequestAborted);

        return RedirectToPage();
    }

    private async Task BuildViewModelAsync()
    {
        var grants = await interaction.GetAllUserGrantsAsync(HttpContext.RequestAborted);

        var list = new List<GrantViewModel>();
        foreach (var grant in grants)
        {
            var client = await clients.FindClientByIdAsync(grant.ClientId, HttpContext.RequestAborted);
            if (client != null)
            {
                var resourcesByScope = await _resources.FindResourcesByScopeAsync(grant.Scopes, HttpContext.RequestAborted);

                var item = new GrantViewModel()
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName ?? client.ClientId,
                    ClientLogoUrl = client.LogoUri,
                    ClientUrl = client.ClientUri,
                    Created = grant.CreationTime,
                    Expires = grant.Expiration,
                    IdentityGrantNames = [.. resourcesByScope.IdentityResources.Select(x => x.DisplayName ?? x.Name)],
                    ApiGrantNames = [.. resourcesByScope.ApiResources.Select(x => x.DisplayName ?? x.Name)]
                };

                list.Add(item);
            }
        }

        Grants = list;
    }

    public class GrantViewModel
    {
        public string? ClientId { get; set; }
        public string? ClientName { get; set; }
        public string? ClientUrl { get; set; }
        public string? ClientLogoUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Expires { get; set; }
        public IEnumerable<string>? IdentityGrantNames { get; set; }
        public IEnumerable<string>? ApiGrantNames { get; set; }
    }
}