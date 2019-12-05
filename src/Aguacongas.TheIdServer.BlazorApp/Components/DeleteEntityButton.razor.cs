using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class DeleteEntityButton
    {
        private string _checkEntityId;

        [Parameter]
        public string EntityId { get; set; }

        [Parameter]
        public EventCallback<string> DeleteConfirmed { get; set; }

        private async Task OnDeleteClicked()
        {
            if (_checkEntityId == EntityId)
            {
                await _jsRuntime.InvokeVoidAsync("bootstrapInteropt.dismissModal", "delete-entity");
                await DeleteConfirmed.InvokeAsync(EntityId)
                    .ConfigureAwait(false);
            }
        }

    }
}
