using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the identity server4 entity framework stores.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="optionsAction">The options action.</param>
        /// <returns></returns>
        public static IServiceCollection AddIdentityServer4EntityFrameworkStores(this IServiceCollection services, Action<DbContextOptionsBuilder> optionsAction = null)
        {
            return services.AddDbContext<ClientContext>(optionsAction)
                .AddDbContext<ResourceContext>(optionsAction)
                .AddTransient<IClientStore, ClientStore>()
                .AddTransient<ICorsPolicyService, CorsPolicyService>()
                .AddTransient<IResourceStore, ResourceStore>()
                .AddTransient<IAuthorizationCodeStore, AuthorizationCodeStore>()
                .AddTransient<IRefreshTokenStore, RefreshTokenStore>()
                .AddTransient<IReferenceTokenStore, ReferenceTokenStore>()
                .AddTransient<IUserConsentStore, UserConsentStore>()
                .AddTransient<IAdminStore<Client>, AdminClientStore>()
                .AddTransient<IGetAllUserConsentStore, GetAllUserConsentStore>();
        }
    }
}
