namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class Spinner
    {
        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
