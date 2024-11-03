using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account.Manage
{
    public class CibaModel : PageModel
    {
        public IEnumerable<BackchannelUserLoginRequest>? Logins { get; set; }

        [BindProperty, Required]
        public string? Id { get; set; }
        [BindProperty, Required]
        public string? Button { get; set; }

        private readonly IBackchannelAuthenticationInteractionService _backchannelAuthenticationInteraction;

        public CibaModel(IBackchannelAuthenticationInteractionService backchannelAuthenticationInteractionService)
        {
            _backchannelAuthenticationInteraction = backchannelAuthenticationInteractionService;
        }

        public async Task OnGet()
        {
            Logins = await _backchannelAuthenticationInteraction.GetPendingLoginRequestsForCurrentUserAsync();
        }
    }
}
