using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.IdentityComponents
{
    public partial class IdentityClaimTypes
    {
        [Parameter]
        public IdentityResource Model { get; set; }

        [Parameter]
        public EventCallback<IdentityClaim> DeleteClaimClicked { get; set; }

        [Parameter]
        public EventCallback<IdentityClaim> ClaimValueChanged { get; set; }

        private bool Validate(string claimType)
        {
            if (Model.IdentityClaims.Any(c => c.Type == claimType))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid claim type",
                    IsError = true,
                    Message = $"This identity already contains the claim type {claimType}"
                });
                return false;
            }
            return true;
        }
    }
}
