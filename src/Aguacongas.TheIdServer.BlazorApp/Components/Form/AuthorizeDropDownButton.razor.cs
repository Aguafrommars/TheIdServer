using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components.Form
{
    public partial class AuthorizeDropDownButton
    {
        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string CssSubClass { get; set; }

        [Parameter]
        public IEnumerable<string> Values { get; set; }

        [Parameter]
        public string SelectedValue { get; set; }

        [Parameter]
        public EventCallback<string> SelectedValueChanged { get; set; }

        private string GetActiveClass(string value)
        {
            return value == SelectedValue ? "active" : null;
        }

        private void SetSelectValue(string value)
        {
            SelectedValue = value;
            SelectedValueChanged.InvokeAsync(value);
            StateHasChanged();
        }
    }
}
