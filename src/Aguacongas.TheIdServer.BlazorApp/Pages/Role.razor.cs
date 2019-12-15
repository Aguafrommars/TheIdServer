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
        private List<RoleClaim> _claims;
        private List<RoleClaim> _claimsState;

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override Entity.Role Create()
        {
            _claims = new List<Entity.RoleClaim>();
            _claimsState = new List<Entity.RoleClaim>();
            return new Entity.Role();
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override async Task<Entity.Role> GetModelAsync()
        {
            var role = await base.GetModelAsync();

            var claimsResponse = await _roleClaimStore.GetAsync(new PageRequest
            {
                Filter = $"RoleId eq '{role.Id}'"
            });

            _claims = claimsResponse.Items.ToList();
            _claimsState = new List<RoleClaim>(_claims);
            
            return role;
        }

        private void OnFilterChanged(string term)
        {
            _claims = _claimsState.Where(c => c.Type.Contains(term) || c.Value.Contains(term))
                .ToList();
        }

        private void OnAddClaimClicked()
        {
            var claim = new RoleClaim();
            _claims.Add(claim);
            _claimsState.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(RoleClaim claim)
        {
            _claimsState.Remove(claim);
            _claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }
    }
}
