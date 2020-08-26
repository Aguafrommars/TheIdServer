// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
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
        protected override string Expand => $"{nameof(entity.User.UserClaims)},{nameof(entity.User.UserRoles)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "users";

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

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
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

            var userLoginStore = GetStore<entity.UserLogin>();
            var getLoginsTask = userLoginStore.GetAsync(pageRequest);

            var userConsentStore = GetStore<entity.UserConsent>();
            var getUserConsentsTask = userConsentStore.GetAsync(pageRequest);
            
            var userTokenStore = GetStore<entity.UserToken>();
            var getUserTokensTask =userTokenStore.GetAsync(pageRequest);

            var referenceTokenStore = GetStore<entity.ReferenceToken>();
            var getReferenceTokenTask = referenceTokenStore.GetAsync(pageRequest);

            var refreshTokenStore = GetStore<entity.RefreshToken>();
            var getRefreshTokenTask = refreshTokenStore.GetAsync(pageRequest);

            var model = await getModelTask.ConfigureAwait(false);
            model.Logins = (await getLoginsTask.ConfigureAwait(false)).Items.ToList();
            model.Consents = (await getUserConsentsTask.ConfigureAwait(false)).Items.ToList();
            model.Tokens = (await getUserTokensTask.ConfigureAwait(false)).Items.ToList();
            model.ReferenceTokens = (await getReferenceTokenTask.ConfigureAwait(false)).Items.ToList();
            model.RefreshTokens = (await getRefreshTokenTask.ConfigureAwait(false)).Items.ToList();

            var userRoles = model.UserRoles;
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

        private entity.UserClaim CreateClaim()
            => new entity.UserClaim
            {
                Issuer = ClaimsIdentity.DefaultIssuer
            };
    }
}
