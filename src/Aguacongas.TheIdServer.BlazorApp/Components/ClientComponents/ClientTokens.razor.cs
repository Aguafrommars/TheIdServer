using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ClientComponents
{
    public partial class ClientTokens
    {
        private readonly Dictionary<string, TimeSpan?> _accessTokenQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["15:00"] = TimeSpan.FromMinutes(15),
            ["30:00"] = TimeSpan.FromMinutes(30),
            ["1:00:00"] = TimeSpan.FromHours(1),
            ["5:00:00"] = TimeSpan.FromHours(5)
        };

        private readonly Dictionary<string, TimeSpan?> _idTokenQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["05:00"] = TimeSpan.FromMinutes(5),
            ["10:00"] = TimeSpan.FromMinutes(10)
        };

        private readonly Dictionary<string, TimeSpan?> _absoluteRefreshTokenQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["1.00:00:00"] = TimeSpan.FromDays(1),
            ["15.00:00:00"] = TimeSpan.FromDays(15),
            ["30.00:00:00"] = TimeSpan.FromDays(30)
        };

        private readonly Dictionary<string, TimeSpan?> _slidingRefreshTokenQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["15:00"] = TimeSpan.FromMinutes(15),
            ["30:00"] = TimeSpan.FromMinutes(30),
            ["1:00:00"] = TimeSpan.FromHours(1),
            ["5:00:00"] = TimeSpan.FromHours(5)
        };

        private readonly Dictionary<string, TimeSpan?> _ssoLifetimeQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["1:00:00"] = TimeSpan.FromDays(15),
            ["5:00:00"] = TimeSpan.FromDays(30),
            ["1.00:00:00"] = TimeSpan.FromDays(365),
            ["Session lifetime"] = null
        };

        private readonly Dictionary<string, TimeSpan?> _deviceCodeLifetimeQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["05:00"] = TimeSpan.FromMinutes(5),
            ["10:00"] = TimeSpan.FromMinutes(10)
        };

        [Parameter]
        public Entity.Client Model { get; set; }

        [Parameter]
        public EventCallback<AccessTokenType> TokenTypeChanged { get; set; }

        [Parameter]
        public EventCallback ModelChanged { get; set; }

        private void SetTokenType(AccessTokenType accessTokenType)
        {
            TokenTypeChanged.InvokeAsync(accessTokenType);
        }

        private void TokenChanded()
        {
            ModelChanged.InvokeAsync(Model);
        }
    }
}
