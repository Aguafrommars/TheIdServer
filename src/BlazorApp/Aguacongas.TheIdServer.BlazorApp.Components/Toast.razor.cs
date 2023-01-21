// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class Toast
    {
        private string BodyClass => Notification.IsError ? "text-danger" : "text-success";
        private string CarretColor => Notification.IsError ? "#dc3545" : "#28a745";
        private string AutoHide => Notification.IsError ? "false" : "true";

        [Parameter]
        public Notification Notification { get; set; }

        [Parameter]
        public EventCallback<Notification> Closed { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
            await _jsRuntime.InvokeVoidAsync("bootstrapInteropt.showToast",
                Notification.Id, DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public Task ToastClosed()
        {
            return Closed.InvokeAsync(Notification);
        }
    }
}
