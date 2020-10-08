using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class KeyCollection
    {
        string _revokeReason;

        [Parameter]
        public EventCallback<Tuple<string, string>> RevokeClick { get; set; }

        string GetItemClass(Key key)
        {
            var defaultClass = key.IsDefault ? "bg-info" : null;
            var revokedClass = key.IsRevoked ? "revoked" : null;
            return $"{defaultClass} {revokedClass}";
        }


        async Task DeleteConfirmed(string id)
        {
            await RevokeClick.InvokeAsync(new Tuple<string, string>(id, _revokeReason));
            _revokeReason = null;
        }

    }
}
