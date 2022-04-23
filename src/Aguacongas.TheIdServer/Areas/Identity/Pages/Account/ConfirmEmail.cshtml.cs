// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer _localizer;

        public ConfirmEmailModel(UserManager<ApplicationUser> userManager, IStringLocalizer<ConfirmEmailModel> localizer)
        {
            _userManager = userManager;
            _localizer = localizer;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(_localizer["Unable to load user with ID '{0}'.", userId]);
            }

            var decodedArray = WebEncoders.Base64UrlDecode(code);
            var decoded = Encoding.UTF8.GetString(decodedArray);
            var result = await _userManager.ConfirmEmailAsync(user, decoded);
            StatusMessage = result.Succeeded ? _localizer["Thank you for confirming your email."] : _localizer["Error confirming your email."];
            return Page();
        }
    }
}
