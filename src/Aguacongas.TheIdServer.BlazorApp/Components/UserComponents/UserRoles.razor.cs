using Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
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

        private bool Validate(string role)
        {
            if (Model.Any(c => c.Name == role))
            {
                _notifier.Notify(new Notification
                {
                    Header = "Invalid role",
                    IsError = true,
                    Message = $"This user already contains the role {role}"
                });
                return false;
            }
            return true;
        }
    }
}
