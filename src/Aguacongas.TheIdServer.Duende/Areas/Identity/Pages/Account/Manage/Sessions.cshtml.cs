using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account.Manage;

public class SessionModel(ISessionManagementService sessionManagementService, IClientStore clients) : PageModel
{
    public QueryResult<UserSession>? Sessions { get; set; }

    [BindProperty, Required]
    public string? Id { get; set; }
    [BindProperty, Required]
    public string? Button { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        Sessions = await sessionManagementService.QuerySessionsAsync(new SessionQuery
        {
            SubjectId = User.FindFirst("sub")!.Value
        }, HttpContext.RequestAborted).ConfigureAwait(false);

        return Page();
    }

    public async Task<string> GetClientNameAsync(string clientId)
    => (await clients.FindClientByIdAsync(clientId, HttpContext.RequestAborted).ConfigureAwait(false))?.ClientName ?? clientId;

    public async Task<IActionResult> OnPostDeleteAsync(string sessionId)
    {
        await sessionManagementService.RemoveSessionsAsync(new RemoveSessionsContext
        {
            SessionId = sessionId
        }, HttpContext.RequestAborted).ConfigureAwait(false);
        return RedirectToPage();
    }

    public string GetActivePageClass(int index)
    => index == Sessions!.CurrentPage ? "active" : string.Empty;

    public string GetPreviousPageClass()
    => Sessions!.HasPrevResults ? string.Empty : "disabled";

    public string GetNextPageClass()
    => Sessions!.HasNextResults ? string.Empty : "disabled";
}
