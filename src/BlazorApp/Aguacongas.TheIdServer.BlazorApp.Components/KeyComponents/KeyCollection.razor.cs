using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components.KeyComponents
{
    public partial class KeyCollection
    {
        [Parameter]
        public EventCallback<Tuple<string, string>> RevokeClick { get; set; }

        string GetItemClass(Key key)
        {
            var defaultClass = key.IsDefault ? "bg-info" : null;
            var revokedClass = key.IsRevoked ? "text-black-50" : null;
            return $"{defaultClass} {revokedClass}";
        }


        private Task RevokeConfirmed(Tuple<string, string> tuple)
            => RevokeClick.InvokeAsync(tuple);

    }
}
