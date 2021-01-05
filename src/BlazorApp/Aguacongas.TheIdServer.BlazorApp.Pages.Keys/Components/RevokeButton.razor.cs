// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Keys.Components
{
    public partial class RevokeButton
    {
        string _revokeReason;

        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Assign by jsScript.")]
        private string _checkEntityId;

        [Parameter]
        public Key Key { get; set; }

        [Parameter]
        public EventCallback<Tuple<string, string>> RevokeConfirmed { get; set; }
        private async Task OnRevokeClicked()
        {
            if (_checkEntityId == Key.Id)
            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
#pragma warning disable CA2012 // Use ValueTasks correctly, execution of the current method continues before the call is completed
                _jsRuntime.InvokeVoidAsync("bootstrapInteropt.dismissModal", $"revoke-entity-{Key.Id}");
#pragma warning restore CA2012 // Use ValueTasks correctly, execution of the current method continues before the call is completed
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                await RevokeConfirmed.InvokeAsync(new Tuple<string, string>(Key.Id, _revokeReason))
                    .ConfigureAwait(false);
            }
        }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
