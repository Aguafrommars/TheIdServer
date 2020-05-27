using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class User
    {
        private readonly GridState _gridState = new GridState();

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "users";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);
            AddEmptyRole();
        }

        protected override Task<Models.User> Create()
        {
            return Task.FromResult(new Models.User
            {
                Claims = new List<entity.UserClaim>(),
                Consents = new List<entity.UserConsent>(),
                Logins = new List<entity.UserLogin>(),
                Roles = new List<entity.Role>(),
                Tokens = new List<entity.UserToken>(),
                ReferenceTokens = new List<entity.ReferenceToken>(),
                RefreshTokens = new List<entity.RefreshToken>()
            });
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is entity.UserClaim claim)
            {
                claim.UserId = Model.Id;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override async Task<Models.User> GetModelAsync()
        {
            var pageRequest = new PageRequest
            {
                Filter = $"{nameof(entity.UserClaim.UserId)} eq '{Id}'"
            };

            var getModelTask = base.GetModelAsync();

            var userClaimStore = GetStore<entity.UserClaim>();
            var getClaimsTask = userClaimStore.GetAsync(pageRequest);

            var userLoginStore = GetStore<entity.UserLogin>();
            var getLoginsTask = userLoginStore.GetAsync(pageRequest);

            var userRoleStore = GetStore<entity.UserRole>();
            var getUserRolesTask = userRoleStore.GetAsync(pageRequest);

            var userConsentStore = GetStore<entity.UserConsent>();
            var getUserConsentsTask = userConsentStore.GetAsync(pageRequest);
            
            var userTokenStore = GetStore<entity.UserToken>();
            var getUserTokensTask =userTokenStore.GetAsync(pageRequest);

            var referenceTokenStore = GetStore<entity.ReferenceToken>();
            var getReferenceTokenTask = referenceTokenStore.GetAsync(pageRequest);

            var refreshTokenStore = GetStore<entity.RefreshToken>();
            var getRefreshTokenTask = refreshTokenStore.GetAsync(pageRequest);

            var model = await getModelTask.ConfigureAwait(false);
            model.Claims = (await getClaimsTask.ConfigureAwait(false)).Items.ToList();
            model.Logins = (await getLoginsTask.ConfigureAwait(false)).Items.ToList();
            model.Consents = (await getUserConsentsTask.ConfigureAwait(false)).Items.ToList();
            model.Tokens = (await getUserTokensTask.ConfigureAwait(false)).Items.ToList();
            model.ReferenceTokens = (await getReferenceTokenTask.ConfigureAwait(false)).Items.ToList();
            model.RefreshTokens = (await getRefreshTokenTask.ConfigureAwait(false)).Items.ToList();

            var userRoles = (await getUserRolesTask.ConfigureAwait(false)).Items;
            if (userRoles.Any())
            {
                var roleStore = GetStore<entity.Role>();
                var rolesResponse = await roleStore.GetAsync(new PageRequest
                {
                    Filter = string.Join(" or ", userRoles.Select(r => $"{nameof(entity.Role.Id)} eq '{r.RoleId}'"))
                }).ConfigureAwait(false);
                model.Roles = rolesResponse.Items.ToList();
            }
            else
            {
                model.Roles = new List<entity.Role>();
            }

            return model;
        }

        protected async override Task<object> CreateAsync(Type entityType, object entity)
        {
            if (entity is entity.Role role)
            {
                var roleStore = GetStore<entity.Role>();
                var roleResponse = await roleStore.GetAsync(new PageRequest
                {
                    Select = "Id",
                    Take = 1,
                    Filter = $"{nameof(role.Name)} eq '{role.Name}'"
                }).ConfigureAwait(false);

                var roles = roleResponse.Items;
                if (roles.Any())
                {
                    await base.CreateAsync(typeof(entity.UserRole), new entity.UserRole
                    {
                        RoleId = roles.First().Id,
                        UserId = Model.Id
                    }).ConfigureAwait(false);
                }
                return role;
            }
            return await base.CreateAsync(entityType, entity).ConfigureAwait(false);
        }

        protected override Task<object> DeleteAsync(Type entityType, object entity)
        {
            if (entity is entity.Role role)
            {
                return base.DeleteAsync(typeof(entity.UserRole), new entity.UserRole
                {
                    Id = $"{Model.Id}@{role.Id}"
                });
            }
            return base.DeleteAsync(entityType, entity);
        }
        private void AddEmptyRole()
        {
            Model.Roles.Add(new entity.Role());
        }

        private void OnFilterChanged(string term)
        {
            Model.Claims = State.Claims.Where(c => c.ClaimType.Contains(term) || c.ClaimValue.Contains(term))
                .ToList();

            Model.Logins = State.Logins.Where(l => l.ProviderDisplayName.Contains(term))
                .ToList();

            Model.Roles = State.Roles.Where(r => r.Name != null && r.Name.Contains(term))
                .ToList();

            Model.Consents = State.Consents.Where(c => c.ClientId.Contains(term))
                .ToList();

            Model.Tokens = State.Tokens.Where(t => t.LoginProvider.Contains(term) || t.Name.Contains(term) || t.Value.Contains(term))
                .ToList();

            Model.RefreshTokens = State.RefreshTokens.Where(t => t.ClientId.Contains(term) || t.Data.Contains(term))
                .ToList();

            Model.ReferenceTokens = State.ReferenceTokens.Where(t => t.ClientId.Contains(term) || t.Data.Contains(term))
                .ToList();

            AddEmptyRole();
        }

        private entity.UserClaim CreateClaim()
            => new entity.UserClaim
            {
                Issuer = ClaimsIdentity.DefaultIssuer
            };

        private void OnDeleteClaimClicked(entity.UserClaim claim)
        {
            Model.Claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }

        private void OnDeleteRoleClicked(entity.Role role)
        {
            Model.Roles.Remove(role);
            EntityDeleted(role);
            StateHasChanged();
        }

        private void OnRoleValueChanged(entity.Role role)
        {
            EntityCreated(role);
            AddEmptyRole();
            StateHasChanged();
        }
    }
}
