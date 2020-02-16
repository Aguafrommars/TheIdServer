using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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
            await base.OnInitializedAsync();
            AddEmptyRole();
        }

        protected override Models.User Create()
        {
            return new Models.User
            {
                Claims = new List<entity.UserClaim>(),
                Consents = new List<entity.UserConsent>(),
                Logins = new List<entity.UserLogin>(),
                Roles = new List<entity.Role>(),
                Tokens = new List<entity.UserToken>()
            };
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
            var model = await base.GetModelAsync();

            var pageRequest = new PageRequest
            {
                Filter = $"{nameof(entity.UserClaim.UserId)} eq '{model.Id}'"
            };

            var userClaimStore = GetStore<entity.UserClaim>();
            var claimsResponse = await userClaimStore.GetAsync(pageRequest);

            model.Claims = claimsResponse.Items.ToList();

            var userLoginStore = GetStore<entity.UserLogin>();
            var loginsRespone = await userLoginStore.GetAsync(pageRequest);

            model.Logins = loginsRespone.Items.ToList();

            var userRoleStore = GetStore<entity.UserRole>();
            var userRolesResponse = await userRoleStore.GetAsync(pageRequest);

            var userRoles = userRolesResponse.Items;
            if (userRoles.Any())
            {
                var roleStore = GetStore<entity.Role>();
                var rolesResponse = await roleStore.GetAsync(new PageRequest
                {
                    Filter = string.Join(" or ", userRolesResponse.Items.Select(r => $"{nameof(entity.Role.Id)} eq '{r.RoleId}'"))
                });
                model.Roles = rolesResponse.Items.ToList();
            }
            else
            {
                model.Roles = new List<entity.Role>();
            }

            var userConsentStore = GetStore<entity.UserConsent>();
            var userConsentsResponse = await userConsentStore.GetAsync(pageRequest);

            model.Consents = userConsentsResponse.Items.ToList();

            var userTokenStore = GetStore<entity.UserToken>();
            var userTokensResponse = await userTokenStore.GetAsync(pageRequest);

            model.Tokens = userTokensResponse.Items.ToList();

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
                });

                var roles = roleResponse.Items;
                if (roles.Any())
                {
                    await base.CreateAsync(typeof(entity.UserRole), new entity.UserRole
                    {
                        RoleId = roles.First().Id,
                        UserId = Model.Id
                    });                   
                }
                return role;
            }
            return await base.CreateAsync(entityType, entity);
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

            AddEmptyRole();
        }

        private void OnAddClaimClicked()
        {
            var claim = new entity.UserClaim();
            State.Claims.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

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

        private void OnDeleteUserTokenClicked(entity.UserToken token)
        {
            Model.Tokens.Remove(token);
            EntityDeleted(token);
            StateHasChanged();
        }

        private void OnDeleteUserLoginClicked(entity.UserLogin login)
        {
            Model.Logins.Remove(login);
            EntityDeleted(login);
            StateHasChanged();
        }

        private void OnDeleteUserConsentClicked(entity.UserConsent consent)
        {
            Model.Consents.Remove(consent);
            EntityDeleted(consent);
            StateHasChanged();
        }
    }
}
