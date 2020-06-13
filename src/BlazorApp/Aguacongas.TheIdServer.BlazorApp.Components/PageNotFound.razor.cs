namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class PageNotFound
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
