using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class User
    {
        private readonly GridState _gridState = new GridState();
        private List<EntityClaim> _claims;
        private List<EntityClaim> _claimsState;
        private List<Login> _logins;
        private List<Login> _loginsState;
        private List<string> _roles;
        private List<string> _rolesState;

        private IIdentityUserStore<IdentityUser> IdentityUserStore => AdminStore as IIdentityUserStore<IdentityUser>;

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override IdentityUser Create()
        {
            _claims = new List<EntityClaim>();
            _claimsState = new List<EntityClaim>();
            return new IdentityUser();
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override IdentityUser CloneModel(IdentityUser entity)
        {
            return new IdentityUser
            {
                Id = entity.Id,
                AccessFailedCount = entity.AccessFailedCount,
                ConcurrencyStamp = entity.ConcurrencyStamp,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                LockoutEnabled = entity.LockoutEnabled,
                LockoutEnd = entity.LockoutEnd,
                NormalizedEmail = entity.NormalizedEmail,
                NormalizedUserName = entity.NormalizedUserName,
                PasswordHash = entity.PasswordHash,
                PhoneNumber = entity.PhoneNumber,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                SecurityStamp = entity.SecurityStamp,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                UserName = entity.UserName
            };
        }

        protected override async Task<IdentityUser> GetModelAsync()
        {
            var user = await base.GetModelAsync();

            var claimsResponse = await IdentityUserStore.GetClaimsAsync(user.Id, null);

            _claims = claimsResponse.Items.ToList();
            _claimsState = new List<EntityClaim>(_claims);

            var loginsRespone = await IdentityUserStore.GetLoginsAsync(user.Id, null);

            _logins = loginsRespone.Items.ToList();
            _loginsState = new List<Login>(_logins);

            var rolesResponse = await IdentityUserStore.GetRolesAsync(user.Id, null);

            _roles = rolesResponse.Items.ToList();
            _rolesState = new List<string>(_roles);

            return user;
        }

        protected override string GetModelId<TEntity>(TEntity model)
        {
            if (model is EntityClaim claim)
            {
                return claim.Type == null && claim.Value == null ? null : $"{claim.Type}/{claim.Value}";
            }
            if (model is Login login)
            {
                return $"{login.LoginProvider}/{login.ProviderKey}";
            }
            if (model is string role)
            {
                return role;
            }
            if (model is IdentityUser user)
            {
                return user.Id;
            }
            return base.GetModelId(model);
        }

        protected override async Task<object> CreateAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                return await IdentityUserStore.AddClaimAsync(Model.Id, entity as EntityClaim);
            }
            if (entityType == typeof(string))
            {
                return await IdentityUserStore.AddRoleAsync(Model.Id, entity as string);
            }
            return base.CreateAsync(entityType, entity);
        }

        protected override async Task<object> UpdateAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                await IdentityUserStore.RemoveClaimAsync(Model.Id, entity as EntityClaim);
                return await IdentityUserStore.AddClaimAsync(Model.Id, entity as EntityClaim);
            }

            return await base.UpdateAsync(entityType, entity);
        }

        protected override async Task<object> DeleteAsync(Type entityType, object entity)
        {
            if (entityType == typeof(EntityClaim))
            {
                await IdentityUserStore.RemoveClaimAsync(Model.Id, entity as EntityClaim);
                return entity;
            }
            if (entityType == typeof(Login))
            {
                await IdentityUserStore.RemoveLoginAsync(Model.Id, entity as Login);
                return entity;
            }
            if (entityType == typeof(string))
            {
                await IdentityUserStore.RemoveRoleAsync(Model.Id, entity as string);
                return entity;
            }
            return await base.DeleteAsync(entityType, entity);
        }

        private void OnFilterChanged(string term)
        {
            _claims = _claimsState.Where(c => c.Type.Contains(term) || c.Value.Contains(term))
                .ToList();

            _logins = _loginsState.Where(l => l.ProviderDisplayName.Contains(term))
                .ToList();

            _roles = _rolesState.Where(r => r.Contains(term))
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
