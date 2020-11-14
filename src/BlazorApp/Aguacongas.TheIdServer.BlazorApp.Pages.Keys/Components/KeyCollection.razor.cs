using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Keys.Components
{
    public partial class KeyCollection
    {
        [Parameter]
        public EventCallback<Tuple<string, string>> RevokeClick { get; set; }

        string GetRevokedClass(Key key)
        {
            return key.IsRevoked ? "text-black-50" : null;
        }


        private Task RevokeConfirmed(Tuple<string, string> tuple)
            => RevokeClick.InvokeAsync(tuple);

    }
}
