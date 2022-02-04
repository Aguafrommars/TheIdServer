// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer;
using Duende.IdentityServer.Models;
#else
using IdentityServer4;
using IdentityServer4.Models;
#endif
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            var profile = new IdentityResources.Profile();
            profile.UserClaims.Add("role");
            return new IdentityResource[]
            {
                profile,
                new IdentityResources.OpenId(),
                new IdentityResources.Address(),
                new IdentityResources.Email(),
                new IdentityResources.Phone(),
            };
        }

        public static IEnumerable<ApiResource> GetApis(IConfiguration configuration)
        {
            var apiList = configuration.GetSection("InitialData:Apis").Get<IEnumerable<ApiResource>>() ?? Array.Empty<ApiResource>();
            foreach (var api in apiList)
            {
                foreach(var secret in api.ApiSecrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret))
                {
                    secret.Value = HashExtensions.Sha256(secret.Value);
                }
                yield return api;
            }
        }

        public static IEnumerable<ApiScope> GetApiScopes(IConfiguration configuration)
        => configuration.GetSection("InitialData:ApiScopes").Get<IEnumerable<ApiScope>>() ?? Array.Empty<ApiScope>();

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            var clientList = configuration.GetSection("InitialData:Clients").Get<IEnumerable<Client>>() ?? Array.Empty<Client>();
            foreach(var client in clientList)
            {
                foreach(var secret in client.ClientSecrets.Where(s => s.Type == IdentityServerConstants.SecretTypes.SharedSecret))
                {
                    secret.Value = HashExtensions.Sha256(secret.Value);
                }
                yield return client;
            }
        }

        public static IEnumerable<Entity.RelyingParty> GetRelyingParties(IConfiguration configuration)
        => configuration.GetSection("InitialData:RelyingParties").Get<IEnumerable<Entity.RelyingParty>>() ?? Array.Empty<Entity.RelyingParty>();
    }
}