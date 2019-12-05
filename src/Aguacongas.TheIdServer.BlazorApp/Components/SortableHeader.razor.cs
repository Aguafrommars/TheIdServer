using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class SortableHeader
    {
        private string _class = null;

        [Parameter]
        public string Property { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public GridState GridState { get; set; }

        [Parameter]
        public EventCallback<string> TermChanged { get; set; }

        void OnHeaderClick()
        {
            GridState.HeaderClicked(Property);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            GridState.OnHeaderClicked += e =>
            {
                _class = GridState.GetHeaderArrowClassSuffix(Property);
                StateHasChanged();
                return Task.CompletedTask;
            };
        }
    }
}
