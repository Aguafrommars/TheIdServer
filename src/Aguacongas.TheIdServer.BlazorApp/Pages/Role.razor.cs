using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class Role
    {
        private GridState _gridState = new GridState();
        private List<EntityClaim> _claims;
        private List<EntityClaim> _claimsState;

        private IIdentityRoleStore<IdentityRole> _store => AdminStore as IIdentityRoleStore<IdentityRole>;

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override IdentityRole Create()
        {
            _claims = new List<EntityClaim>();
            _claimsState = new List<EntityClaim>();
            return new IdentityRole();
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override IdentityRole CloneModel(IdentityRole entity)
        {
            return new IdentityRole
            {
                ConcurrencyStamp = entity.ConcurrencyStamp,
                Id = entity.Id,
                Name = entity.Name,
                NormalizedName = entity.NormalizedName
            };
        }

        protected override async Task<IdentityRole> GetModelAsync()
        {
            var role = await base.GetModelAsync();

            var claimsResponse = await _store.GetClaimsAsync(role.Id, null);

            _claims = claimsResponse.Items.ToList();
            _claimsState = new List<EntityClaim>(_claims);
            
            return role;
        }

        protected override string GetModelId<TEntity>(TEntity model)
        {
            if (model is EntityClaim claim)
            {
                return claim.Type == null && claim.Value == null ? null : $"{claim.Type}/{claim.Value}";
            }
            if (model is IdentityRole role)
            {
                return role.Id;
            }
            return base.GetModelId(model);
        }

        protected override async Task<object> CreateAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                return await _store.AddClaimAsync(Model.Id, entity as EntityClaim);
            }
            return base.CreateAsync(entityType, entity);
        }

        protected override async Task<object> UpdateAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                await _store.RemoveClaimAsync(Model.Id, entity as EntityClaim);
                return await _store.AddClaimAsync(Model.Id, entity as EntityClaim);
            }

            return await base.UpdateAsync(entityType, entity);
        }

        protected override async Task<object> DeleteAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                await _store.RemoveClaimAsync(Model.Id, entity as EntityClaim);
                return entity;
            }
            return await base.DeleteAsync(entityType, entity);
        }

        private void OnFilterChanged(string term)
        {
            _claims = _claimsState.Where(c => c.Type.Contains(term) || c.Value.Contains(term))
                .ToList();
        }

        private void OnAddClaimClicked()
        {
            var claim = new EntityClaim();
            _claims.Add(claim);
            _claimsState.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(EntityClaim claim)
        {
            _claimsState.Remove(claim);
            _claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }
    }
}
