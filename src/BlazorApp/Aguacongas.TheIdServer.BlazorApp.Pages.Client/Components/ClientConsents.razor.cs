// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
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

        [CascadingParameter]
        public HandleModificationState HandleModificationState { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }

        private Task AddResource(Entity.EntityResourceKind kind)
        {
            var entity = new Entity.ClientLocalizedResource
            {
                ResourceKind = kind
            };
            Model.Resources.Add(entity);
            HandleModificationState.EntityCreated(entity);
            return Task.CompletedTask;
        }
    }
}
