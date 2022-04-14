// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientTokens
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Used in component")]
        private bool _showAllOptions;

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

        private readonly Dictionary<string, TimeSpan?> _cibaLifetimeQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["05:00"] = TimeSpan.FromMinutes(5),
            ["10:00"] = TimeSpan.FromMinutes(10)
        };

        private readonly Dictionary<string, TimeSpan?> _pollingIntervalQuickValues = new Dictionary<string, TimeSpan?>
        {
            ["00:05"] = TimeSpan.FromSeconds(5),
            ["00:10"] = TimeSpan.FromSeconds(10)
        };

        [Parameter]
        public Entity.Client Model { get; set; }


        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private void SetTokenType(AccessTokenType accessTokenType)
        {
            Model.AccessTokenType = (int)accessTokenType;
            TokenChanded();
        }

        private void SetRefreshTokenUsage(RefreshTokenUsage usage)
        {
            Model.RefreshTokenUsage = (int)usage;
            TokenChanded();
        }

        private void SetRefreshTokenExpiration(RefreshTokenExpiration expiration)
        {
            Model.RefreshTokenExpiration = (int)expiration;
            TokenChanded();
        }

        private void TokenChanded()
        {
            HandleModificationState.EntityUpdated(Model);
        }
    }
}
