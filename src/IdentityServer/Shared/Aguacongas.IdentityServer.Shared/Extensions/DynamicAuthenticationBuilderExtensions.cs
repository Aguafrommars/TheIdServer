// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DynamicAuthenticationBuilderExtensions
    {
        public static DynamicAuthenticationBuilder AddNotifyChangedExternalProviderStore<TStore>(this DynamicAuthenticationBuilder builder)
            where TStore: IAdminStore<ExternalProvider>
        {
            builder.Services.AddTransient<IAdminStore<ExternalProvider>>(p =>
            {
                var store = p.GetRequiredService<TStore>();

                return new NotifyChangedExternalProviderStore<TStore>(
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
