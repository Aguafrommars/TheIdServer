
namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class PageLoading
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
