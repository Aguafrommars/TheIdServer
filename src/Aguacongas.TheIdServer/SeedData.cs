// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.Data;
using Aguacongas.TheIdServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer
{
#pragma warning disable S1118 // Utility classes should not have public constructors
    public static class SeedData
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        public static void EnsureSeedData(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.AddLogging()
                .AddDbContext<ApplicationDbContext>(options => options.UseDatabaseFromConfiguration(configuration))
                .AddConfigurationEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(configuration))
                .AddOperationalEntityFrameworkStores(options => options.UseDatabaseFromConfiguration(configuration))
                .AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer()
                .AddAspNetIdentity<ApplicationUser>();

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();

            var dbType = configuration.GetValue<DbTypes>("DbType");
            if (dbType != DbTypes.InMemory)
            {
                var configContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                configContext.Database.Migrate();

                var opContext = scope.ServiceProvider.GetRequiredService<OperationalDbContext>();
                opContext.Database.Migrate();

                var appcontext = scope.ServiceProvider.GetService<ApplicationDbContext>();
                appcontext.Database.Migrate();
            }

            SeedUsers(scope);
            SeedConfiguration(scope);
        }

        public static void SeedConfiguration(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                    Console.WriteLine($"Add client {client.ClientName}");
                }
            }

            if (!context.Identities.Any())
            {
                foreach (var resource in Config.GetIdentityResources())
                {
                    context.Identities.Add(resource.ToEntity());
                    Console.WriteLine($"Add identity resource {resource.DisplayName}");
                }
            }

            if (!context.Apis.Any())
            {
                foreach (var resource in Config.GetApis())
                {
                    context.Apis.Add(resource.ToEntity());
                    Console.WriteLine($"Add api resource {resource.DisplayName}");
                }
            }
            context.SaveChanges();
        }

        public static void SeedUsers(IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new string[]
            {
                SharedConstants.WRITER,
                SharedConstants.READER
            };
            foreach (var role in roles)
            {
                if (roleMgr.FindByNameAsync(role).GetAwaiter().GetResult() == null)
                {
                    ExcuteAndCheckResult(() => roleMgr.CreateAsync(new IdentityRole
                    {
                        Name = role
                    })).GetAwaiter().GetResult();
                }
            }

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "alice@theidserver.com",
                    EmailConfirmed = true,
                };
                ExcuteAndCheckResult(() => userMgr.CreateAsync(alice, "Pass123$"))
                    .GetAwaiter().GetResult();

                ExcuteAndCheckResult(() => userMgr.AddClaimsAsync(alice, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Alice Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Alice"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "alice@theidserver.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                    })).GetAwaiter().GetResult();

                ExcuteAndCheckResult(() => userMgr.AddToRolesAsync(alice, roles))
                    .GetAwaiter().GetResult();

                Console.WriteLine("alice created");
            }
            else
            {
                Console.WriteLine("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").GetAwaiter().GetResult();
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "bob@theidserver.com",
                    EmailConfirmed = true
                };
                ExcuteAndCheckResult(() => userMgr.CreateAsync(bob, "Pass123$"))
                    .GetAwaiter().GetResult();

                ExcuteAndCheckResult(() => userMgr.AddClaimsAsync(bob, new Claim[]{
                        new Claim(JwtClaimTypes.Name, "Bob Smith"),
                        new Claim(JwtClaimTypes.GivenName, "Bob"),
                        new Claim(JwtClaimTypes.FamilyName, "Smith"),
                        new Claim(JwtClaimTypes.Email, "bob@theidserver.com"),
                        new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                        new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                        new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                        new Claim("location", "somewhere")
                    })).GetAwaiter().GetResult();
                ExcuteAndCheckResult(() => userMgr.AddToRoleAsync(bob, SharedConstants.READER))
                    .GetAwaiter().GetResult();
                Console.WriteLine("bob created");
            }
            else
            {
                Console.WriteLine("bob already exists");
            }
            context.SaveChanges();
        }

        [SuppressMessage("Major Code Smell", "S112:General exceptions should never be thrown", Justification = "Seeding")]
        private static async Task ExcuteAndCheckResult(Func<Task<IdentityResult>> action)
        {
            var result = await action.Invoke();
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

        }
    }
}
