using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Role
    {
        private readonly GridState _gridState = new GridState();

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override Task<Models.Role> Create()
        {
            return Task.FromResult(new Models.Role
            {
                Claims =new List<RoleClaim>()
            });
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is RoleClaim claim)
            {
                claim.RoleId = Model.Id;
            }
        }

        protected override async Task<Models.Role> GetModelAsync()
        {
            var role = await base.GetModelAsync().ConfigureAwait(false);

            var claimsResponse = await _roleClaimStore.GetAsync(new PageRequest
            {
                Filter = $"{nameof(RoleClaim.RoleId)} eq '{role.Id}'"
            }).ConfigureAwait(false);

            role.Claims = claimsResponse.Items.ToList();
            
            return role;
        }

        protected override Task OnFilterChanged(string term)
        {
            Model.Claims = State.Claims.Where(c => c.ClaimType.Contains(term) || c.ClaimValue.Contains(term))
                .ToList();
            return Task.CompletedTask;
        }

        private RoleClaim CreateClaim()
            => new RoleClaim();

        private void OnDeleteClaimClicked(RoleClaim claim)
        {
            Model.Claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }
    }
}
