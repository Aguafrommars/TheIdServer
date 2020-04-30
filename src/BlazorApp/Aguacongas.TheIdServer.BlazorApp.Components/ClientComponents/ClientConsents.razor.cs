using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientConsents
    {
        private readonly Dictionary<string, TimeSpan?> _consentLifetimeQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["15.00:00:00"] = TimeSpan.FromDays(15),
            ["30.00:00:00"] = TimeSpan.FromDays(30),
            ["365.00:00:00"] = TimeSpan.FromDays(365),
            ["Never expire"] = null
        };

        [Parameter]
        public Entity.Client Model { get; set; }
    }
}
