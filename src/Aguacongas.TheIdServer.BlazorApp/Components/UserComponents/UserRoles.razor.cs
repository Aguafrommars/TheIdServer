using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.UserComponents
{
    public partial class UserRoles
    {
        [Parameter]
        public IEnumerable<Entity.Role> Model { get; set; }

        [Parameter]
        public EventCallback<Entity.Role> DeleteRoleClicked { get; set; }

        [Parameter]
        public EventCallback<Entity.Role> RoleValueChanged { get; set; }
    }
}
