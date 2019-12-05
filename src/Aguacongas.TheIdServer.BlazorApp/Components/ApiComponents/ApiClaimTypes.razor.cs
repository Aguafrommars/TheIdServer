using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ApiComponents
{
    public partial class ApiClaimTypes
    {
        [Inject]
        private Notifier Notifier { get; set; }


        [Parameter]
        public ProtectResource Model { get; set; }

        [Parameter]
        public EventCallback<ApiClaim> DeleteClaimClicked { get; set; }

        [Parameter]
        public EventCallback<ApiClaim> ClaimValueChanged { get; set; }

        private bool Validate(string claimType)
        {
            if (Model.ApiClaims.Any(c => c.Type == claimType))
            {
                Notifier.Notify(new Notification
                {
                    Header = "Invalid claim type",
                    IsError = true,
                    Message = $"This API already contains the claim type {claimType}"
                });
                return false;
            }
            return true;
        }
    }
}
