// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using IdentityServer4;

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
        {
            return configuration.GetSection("InitialData:ApiScopes").Get<IEnumerable<ApiScope>>() ?? Array.Empty<ApiScope>();
        }

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
    }
}