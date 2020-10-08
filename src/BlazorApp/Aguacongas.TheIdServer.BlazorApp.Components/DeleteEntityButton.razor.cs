// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class DeleteEntityButton
    {
        [SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Assign by jsScript.")]
        private string _checkEntityId;

        [Parameter]
        public string EntityId { get; set; }

        [Parameter]
        public EventCallback<string> DeleteConfirmed { get; set; }

        [Parameter]
        public string WarningText { get; set; } = "Retype the \"{0}\" if you are sure to delete it.";

        [Parameter]
        public RenderFragment PopupAdditionalDetail { get; set; }

        private async Task OnDeleteClicked()
        {
            if (_checkEntityId == EntityId)
            {
                await _jsRuntime.InvokeVoidAsync("bootstrapInteropt.dismissModal", "delete-entity");
                await DeleteConfirmed.InvokeAsync(EntityId)
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
