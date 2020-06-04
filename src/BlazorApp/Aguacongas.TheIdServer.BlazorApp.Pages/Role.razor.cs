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

        private IEnumerable<RoleClaim> Claims => Model.Claims.Where(c => c.Id == null || (c.ClaimType != null && c.ClaimType.Contains(HandleModificationState.FilterTerm)) || (c.ClaimValue != null && c.ClaimValue.Contains(HandleModificationState.FilterTerm)));

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            HandleModificationState.OnFilterChange += HandleModificationState_OnFilterChange;
            HandleModificationState.OnStateChange += HandleModificationState_OnStateChange;
        }

        private void HandleModificationState_OnStateChange(ModificationKind kind, object entity)
        {
            StateHasChanged();
        }

        private void HandleModificationState_OnFilterChange(string obj)
        {
            StateHasChanged();
        }

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

        private RoleClaim CreateClaim()
            => new RoleClaim();

        private void OnDeleteClaimClicked(RoleClaim claim)
        {
            Model.Claims.Remove(claim);
            EntityDeleted(claim);
        }
    }
}
