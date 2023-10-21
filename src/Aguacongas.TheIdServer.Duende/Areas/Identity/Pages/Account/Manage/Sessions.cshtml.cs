using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account.Manage
{
    public class SessionModel : PageModel
    {
        public QueryResult<UserSession>? Sessions { get; set; }

        [BindProperty, Required]
        public string? Id { get; set; }
        [BindProperty, Required]
        public string? Button { get; set; }

        private readonly ISessionManagementService _sessionManagementService;
        private readonly IClientStore _clients;

        public SessionModel(ISessionManagementService sessionManagementService, IClientStore clients)
        {
            _sessionManagementService = sessionManagementService;
            _clients = clients;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            Sessions = await _sessionManagementService.QuerySessionsAsync(new SessionQuery
            {
                SubjectId = User.FindFirst("sub")!.Value
            }).ConfigureAwait(false);

            return Page();
        }

        public async Task<string> GetClientNameAsync(string clientId)
        => (await _clients.FindClientByIdAsync(clientId).ConfigureAwait(false))?.ClientName ?? clientId;

        public async Task<IActionResult> OnPostDeleteAsync(string sessionId)
        {
            await _sessionManagementService.RemoveSessionsAsync(new RemoveSessionsContext
            {
                SessionId = sessionId
            }).ConfigureAwait(false);
            return RedirectToPage();
        }

        public string GetActivePageClass(int index)
        => index == Sessions!.CurrentPage ? "active" : string.Empty;

        public string GetPreviousPageClass()
        => Sessions!.HasPrevResults ? string.Empty : "disabled";

        public string GetNextPageClass()
        => Sessions!.HasNextResults ? string.Empty : "disabled";
    }
}
