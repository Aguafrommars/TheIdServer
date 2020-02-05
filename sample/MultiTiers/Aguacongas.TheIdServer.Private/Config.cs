// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using static IdentityServer4.IdentityServerConstants;

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

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api1", "My API #1"),
                new ApiResource("theidserveradminapi", "TheIdServer admin API", new string[] 
                {
                    "name",
                    "role"
                })
                {
                    ApiSecrets = new List<Secret>
                    {
                        new Secret()
                        {
                            Type = SecretTypes.SharedSecret,
                            Value = "5b556f7c-b3bc-4b5b-85ab-45eed0cb962d".Sha256(),
                        }
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientClaimsPrefix = null,
                    ClientId = "client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1" }
                },

                // MVC client using hybrid flow
                new Client
                {
                    ClientClaimsPrefix = null,
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:5001/signin-oidc", "https://localhost:5446/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5001/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5001/signout-callback-oidc", "http://localhost:5446/signout-callback-oidc" },
                    
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientClaimsPrefix = null,
                    ClientId = "spa",
                    ClientName = "SPA Client",
                    ClientUri = "http://localhost:5002",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "http://localhost:5002"
                    },

                    PostLogoutRedirectUris = { "http://localhost:5002" },
                    AllowedCorsOrigins = { "http://localhost:5002" },

                    AllowedScopes = { "openid", "profile", "api1" }
                },

                // SPA client using code flow + pkce
                new Client
                {
                    ClientClaimsPrefix = null,
                    ClientId = "theidserveradmin",
                    ClientName = "TheIdServer admin SPA Client",
                    ClientUri = "https://localhost:5443/admin/",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =
                    {
                        "https://localhost:5443/admin/",
                        "http://exemple.com/"
                    },

                    PostLogoutRedirectUris = 
                    { 
                        "https://localhost:5443/admin/",
                        "http://exemple.com/"
                    },
                    AllowedCorsOrigins = 
                    {
                        "https://localhost:5443",
                        "http://exemple.com/"
                    },
                    AllowedScopes = { "openid", "profile", "theidserveradminapi" },
                    ClientSecrets = new List<Secret>
                    {
                        new Secret()
                        {
                            Type = SecretTypes.SharedSecret,
                            Value = "5b556f7c-b3bc-4b5b-85ab-45eed0cb962d".Sha256(),
                        }
                    }
                },

                new Client
                {
                    ClientClaimsPrefix = null,
                    ClientId = "public-server",
                    ClientName = "Public server Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("84137599-13d6-469c-9376-9e372dd2c1bd".Sha256()) },

                    AllowedScopes = { "theidserveradminapi" },
                    Claims = new List<Claim>
                    {
                        new Claim("role", "Is4-Writer"),
                        new Claim("role", "Is4-Reader"),
                        new Claim("name", "public-server")
                    }
                },

            };
        }
    }
}