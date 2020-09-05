// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
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

        private IEnumerable<Entity.RoleClaim> Claims => Model.Claims.Where(c => c.Id == null || (c.ClaimType != null && c.ClaimType.Contains(HandleModificationState.FilterTerm)) || (c.ClaimValue != null && c.ClaimValue.Contains(HandleModificationState.FilterTerm)));

        protected override string Expand => nameof(Entity.Role.RoleClaims);

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
                Claims =new List<Entity.RoleClaim>()
            });
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.RoleClaim claim)
            {
                claim.RoleId = Model.Id;
            }
        }

        private Entity.RoleClaim CreateClaim()
            => new Entity.RoleClaim();

        private void OnDeleteClaimClicked(Entity.RoleClaim claim)
        {
            Model.Claims.Remove(claim);
            EntityDeleted(claim);
        }
    }
}
