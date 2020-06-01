using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class Filter
    {
        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public string Term { get; set; }

        [Parameter]
        public EventCallback<string> TermChanged { get; set; }

        [Parameter]
        public EventCallback<bool> FocusChanged { get; set; }

        protected override void OnInitialized()
        {
            Localizer.OnResourceReady = () => InvokeAsync(StateHasChanged);
            base.OnInitialized();
        }
        private Task OnTermChanged(ChangeEventArgs e)
        {
            Term = e.Value.ToString();

            return TermChanged.InvokeAsync(Term);
        }
    }
}
