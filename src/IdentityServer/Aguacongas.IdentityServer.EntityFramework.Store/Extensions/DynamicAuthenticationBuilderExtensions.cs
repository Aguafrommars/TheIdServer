// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.EntityFramework.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamicAuthenticationBuilderExtensions
    {
        public static DynamicAuthenticationBuilder AddTheIdServerEntityFrameworkStore(this DynamicAuthenticationBuilder builder)
        {
            return builder.AddTheIdServerEntityFrameworkStore<SchemeDefinition>();
        }

        public static DynamicAuthenticationBuilder AddTheIdServerEntityFrameworkStore<TSchemeDefinition>(this DynamicAuthenticationBuilder builder)
            where TSchemeDefinition : SchemeDefinitionBase, new()
        {
            builder.AddTheIdServerStore<TSchemeDefinition>();
            var services = builder.Services;
            services.AddTransient<IAdminStore<ExternalProvider>>(p =>
            {
                var store = p.GetRequiredService<CacheAdminStore<AdminStore<ExternalProvider, ConfigurationDbContext>, ExternalProvider>>();

                return new NotifyChangedExternalProviderStore<CacheAdminStore<AdminStore<ExternalProvider, ConfigurationDbContext>, ExternalProvider>>(
                    store,
                    p.GetRequiredService<IProviderClient>(),
                    new PersistentDynamicManager<SchemeDefinition>(
                        p.GetRequiredService<IAuthenticationSchemeProvider>(),
                        p.GetRequiredService<OptionsMonitorCacheWrapperFactory>(),
                        new DynamicProviderStore<SchemeDefinition>(
                            store,
                            p.GetRequiredService<IAuthenticationSchemeOptionsSerializer>()),
                        builder.HandlerTypes),
                    p.GetRequiredService<IAuthenticationSchemeOptionsSerializer>());
            });
            return builder;            
        }
    }
}
