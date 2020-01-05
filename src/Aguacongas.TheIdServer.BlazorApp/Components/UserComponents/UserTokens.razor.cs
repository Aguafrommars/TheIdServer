using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.UserComponents
{
    public partial class UserTokens
    {
        private GridState GridState { get; } = new GridState();

        [Parameter]
        public IEnumerable<Entity.UserToken> Model { get; set; }
    }
}
