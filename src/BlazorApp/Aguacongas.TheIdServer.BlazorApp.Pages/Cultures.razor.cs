namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Cultures
    {
        protected override string SelectProperties => "Id";

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
    }
}
