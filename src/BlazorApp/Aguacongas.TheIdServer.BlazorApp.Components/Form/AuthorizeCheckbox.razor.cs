using Microsoft.AspNetCore.Components;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeCheckbox
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Label { get; set; }

        private void Togle()
        {
            Value = !Value;
            EditContext.NotifyFieldChanged(FieldIdentifier);
        }
    }
}
