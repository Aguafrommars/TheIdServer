using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Role
    {
        private readonly GridState _gridState = new GridState();

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override Models.Role Create()
        {
            return new Models.Role
            {
                Claims =new List<Entity.RoleClaim>()
            };
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override async Task<Models.Role> GetModelAsync()
        {
            var role = await base.GetModelAsync();

            var claimsResponse = await _roleClaimStore.GetAsync(new PageRequest
            {
                Filter = $"RoleId eq '{role.Id}'"
            });

            role.Claims = claimsResponse.Items.ToList();
            
            return role;
        }

        private void OnFilterChanged(string term)
        {
            Model.Claims = State.Claims.Where(c => c.Type.Contains(term) || c.Value.Contains(term))
                .ToList();
        }

        private void OnAddClaimClicked()
        {
            var claim = new RoleClaim();
            Model.Claims.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(RoleClaim claim)
        {
            Model.Claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }
    }
}
