using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages
{
    public partial class User
    {
        private readonly GridState _gridState = new GridState();
        private List<Entity.UserClaim> _claims;
        private List<Entity.UserClaim> _claimsState;
        private List<Entity.UserLogin> _logins;
        private List<Entity.UserLogin> _loginsState;
        private List<Entity.Role> _roles;
        private List<Entity.Role> _rolesState;
        private List<Entity.UserConsent> _consents;
        private List<Entity.UserConsent> _consentsState;
        private List<Entity.UserToken> _tokens;
        private List<Entity.UserToken> _tokensState;

        protected override string Expand => null;

        protected override bool NonEditable => false;

        protected override string BackUrl => "roles";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            AddEmptyRole();
        }

        protected override Entity.User Create()
        {
            _claims = new List<Entity.UserClaim>();
            _claimsState = new List<Entity.UserClaim>();
            _logins = new List<Entity.UserLogin>();
            _loginsState = new List<Entity.UserLogin>();
            _roles = new List<Entity.Role>();
            _rolesState = new List<Entity.Role>();
            _consents = new List<Entity.UserConsent>();
            _consentsState = new List<Entity.UserConsent>();
            _tokens = new List<Entity.UserToken>();
            _tokensState = new List<Entity.UserToken>();
            return new Entity.User();
        }

        protected override void SetNavigationProperty<TEntity>(TEntity entity)
        {
            // nothing to do
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            // no navigation property
        }

        protected override async Task<Entity.User> GetModelAsync()
        {
            var user = await base.GetModelAsync();

            var pageRequest = new PageRequest
            {
                Filter = $"UserId eq '{user.Id}'"
            };

            var userClaimStore = GetStore<Entity.UserClaim>();
            var claimsResponse = await userClaimStore.GetAsync(pageRequest);

            _claims = claimsResponse.Items.ToList();
            _claimsState = new List<Entity.UserClaim>(_claims);

            var userLoginStore = GetStore<Entity.UserLogin>();
            var loginsRespone = await userLoginStore.GetAsync(pageRequest);

            _logins = loginsRespone.Items.ToList();
            _loginsState = new List<Entity.UserLogin>(_logins);

            var userRoleStore = GetStore<Entity.UserRole>();
            var userRolesResponse = await userRoleStore.GetAsync(pageRequest);

            var userRoles = userRolesResponse.Items;
            if (userRoles.Any())
            {
                var userStore = GetStore<Entity.Role>();
                var rolesResponse = await userStore.GetAsync(new PageRequest
                {
                    Filter = string.Join(" or ", userRolesResponse.Items.Select(r => $"Id eq '{r.RoleId}'"))
                });
                _roles = rolesResponse.Items.ToList();
            }
            else
            {
                _roles = new List<Entity.Role>();
            }
            _rolesState = new List<Entity.Role>(_roles);

            var userConsentStore = GetStore<Entity.UserConsent>();
            var userConsentsResponse = await userConsentStore.GetAsync(pageRequest);

            _consents = userConsentsResponse.Items.ToList();
            _consentsState = new List<Entity.UserConsent>(_consents);

            var userTokenStore = GetStore<Entity.UserToken>();
            var userTokensResponse = await userTokenStore.GetAsync(pageRequest);

            _tokens = userTokensResponse.Items.ToList();
            _tokensState = new List<Entity.UserToken>(_tokens);
            return user;
        }

        protected async override Task<object> CreateAsync(Type entityType, object entity)
        {
            if (entity is Entity.Role role)
            {
                var roleStore = GetStore<Entity.Role>();
                var roleResponse = await roleStore.GetAsync(new PageRequest
                {
                    Select = "Id",
                    Take = 1,
                    Filter = $"Name eq '{role.Name}'"
                });

                var roles = roleResponse.Items;
                if (roles.Any())
                {
                    await base.CreateAsync(typeof(Entity.UserRole), new Entity.UserRole
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
            if (entity is Entity.Role role)
            {
                return base.DeleteAsync(typeof(Entity.UserRole), new Entity.UserRole
                {
                    Id = $"{Model.Id}@{role.Id}"
                });
            }
            return base.DeleteAsync(entityType, entity);
        }
        private void AddEmptyRole()
        {
            _roles.Add(new Entity.Role());
        }

        private void OnFilterChanged(string term)
        {
            _claims = _claimsState.Where(c => c.Type.Contains(term) || c.Value.Contains(term))
                .ToList();

            _logins = _loginsState.Where(l => l.ProviderDisplayName.Contains(term))
                .ToList();

            _roles = _rolesState.Where(r => r.Name.Contains(term))
                .ToList();

            _consents = _consentsState.Where(c => c.ClientId.Contains(term))
                .ToList();

            _tokens = _tokensState.Where(t => t.LoginProvider.Contains(term) || t.Name.Contains(term) || t.Value.Contains(term))
                .ToList();

            AddEmptyRole();
        }

        private void OnAddClaimClicked()
        {
            var claim = new Entity.UserClaim
            {
                UserId = Model.Id
            };
            _claims.Add(claim);
            _claimsState.Add(claim);
            EntityCreated(claim);
            StateHasChanged();
        }

        private void OnDeleteClaimClicked(Entity.UserClaim claim)
        {
            _claimsState.Remove(claim);
            _claims.Remove(claim);
            EntityDeleted(claim);
            StateHasChanged();
        }

        private void OnDeleteRoleClicked(Entity.Role role)
        {
            _rolesState.Remove(role);
            _roles.Remove(role);
            EntityDeleted(role);
            StateHasChanged();
        }

        private void OnRoleValueChanged(Entity.Role role)
        {
            EntityCreated(role);
            AddEmptyRole();
            StateHasChanged();
        }

        private void OnDeleteUserTokenClicked(Entity.UserToken token)
        {
            _tokensState.Remove(token);
            _tokens.Remove(token);
            EntityDeleted(token);
            StateHasChanged();
        }

        private void OnDeleteUserLoginClicked(Entity.UserLogin login)
        {
            _loginsState.Remove(login);
            _logins.Remove(login);
            EntityDeleted(login);
            StateHasChanged();
        }

        private void OnDeleteUserConsentClicked(Entity.UserConsent consent)
        {
            _consentsState.Remove(consent);
            _consents.Remove(consent);
            EntityDeleted(consent);
            StateHasChanged();
        }
    }
}
