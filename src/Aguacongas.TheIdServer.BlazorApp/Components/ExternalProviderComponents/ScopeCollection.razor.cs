using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components.ExternalProviderComponents
{
    public partial class ScopeCollection
    {
        private string _value;

        [Parameter]
        public string Id { get; set; }

        [Parameter]
        public string Placeholder { get; set; }

        [Parameter]
        public ICollection<string> Collection { get; set; }

        private void OnDelete(string item)
        {
            Collection.Remove(item);
        }

        private void OnInput(ChangeEventArgs e)
        {
            _value = e.Value as string;
        }

        private void OnInputChanged(ChangeEventArgs e)
        {
            OnInput(e);
            AddValue();
        }

        private void AddValue()
        {
            if (_value == null)
            {
                return;
            }
            Collection.Add(_value);
            _value = null;
        }
    }
}
