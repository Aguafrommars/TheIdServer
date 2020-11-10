// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
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
using System.Collections.Generic;
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

            SeedUsers(scope, configuration);
            SeedConfiguration(scope, configuration);
        }

        public static void SeedConfiguration(IServiceScope scope, IConfiguration configuration)
        {
            var provider = scope.ServiceProvider;

            var context = provider.GetRequiredService<ConfigurationDbContext>();

            if (!context.Clients.Any())
            {
                foreach (var client in Config.GetClients(configuration))
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

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.GetApiScopes(configuration))
                {
                    context.ApiScopes.Add(resource.ToEntity());
                    Console.WriteLine($"Add api scope resource {resource.DisplayName}");
                }
            }

            if (!context.Apis.Any())
            {
                foreach (var resource in Config.GetApis(configuration))
                {
                    context.Apis.Add(resource.ToEntity());
                    Console.WriteLine($"Add api resource {resource.DisplayName}");
                }
            }

            context.SaveChanges();
        }

        public static void SeedUsers(IServiceScope scope, IConfiguration configuration)
        {
            var provider = scope.ServiceProvider;

            var context = provider.GetService<ApplicationDbContext>();
            
            var roleMgr = provider.GetRequiredService<RoleManager<IdentityRole>>();

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

            var userMgr = provider.GetRequiredService<UserManager<ApplicationUser>>();
            var userList = configuration.GetSection("InitialData:Users").Get<IEnumerable<ApplicationUser>>();
            int index = 0;
            foreach(var user in userList)
            {
                var existing = userMgr.FindByNameAsync(user.UserName).GetAwaiter().GetResult();
                if (existing != null)
                {
                    Console.WriteLine($"{user.UserName} already exists");
                    continue;
                }
                var pwd = configuration.GetValue<string>($"InitialData:Users:{index}:Password");
                ExcuteAndCheckResult(() => userMgr.CreateAsync(user, pwd))
                    .GetAwaiter().GetResult();

                var claimList = configuration.GetSection($"InitialData:Users:{index}:Claims").Get<IEnumerable<UserClaim>>()
                    .Select(c => c.ToClaim())
                    .ToList();
                claimList.Add(new Claim(JwtClaimTypes.UpdatedAt, DateTime.Now.ToEpochTime().ToString(), ClaimValueTypes.Integer64));
                ExcuteAndCheckResult(() => userMgr.AddClaimsAsync(user, claimList))
                    .GetAwaiter().GetResult();

                var roleList = configuration.GetSection($"InitialData:Users:{index}:Roles").Get<IEnumerable<string>>();
                ExcuteAndCheckResult(() => userMgr.AddToRolesAsync(user, roleList))
                    .GetAwaiter().GetResult();

                Console.WriteLine($"{user.UserName} created");

                index++;
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
