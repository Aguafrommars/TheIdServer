// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdminHttpStores(this IServiceCollection services, Func<IServiceProvider, Task<HttpClient>> getHttpClient)
        {
            var entityTypeList = GetEntityTypes();

            foreach (var entityType in entityTypeList)
            {
                AddHttpAdminStore(services, entityType, getHttpClient);
            }

            return services.AddTransient<IExternalProviderKindStore>(
                    p => new ExternalProviderKindStore(getHttpClient.Invoke(p),
                        p.GetRequiredService<ILogger<ExternalProviderKindStore>>()))
                .AddTransient<IKeyStore<IAuthenticatedEncryptorDescriptor>>(p => new  KeyStore<IAuthenticatedEncryptorDescriptor>(getHttpClient.Invoke(p),
                        p.GetRequiredService<ILogger<KeyStore<IAuthenticatedEncryptorDescriptor>>>()))
                .AddTransient<IKeyStore<RsaEncryptorDescriptor>>(p => new KeyStore<RsaEncryptorDescriptor>(getHttpClient.Invoke(p),
                        p.GetRequiredService<ILogger<KeyStore<RsaEncryptorDescriptor>>>()))
                .AddTransient<IKeyStore<ECDsaEncryptorDescriptor>>(p => new KeyStore<ECDsaEncryptorDescriptor>(getHttpClient.Invoke(p),
                        p.GetRequiredService<ILogger<KeyStore<ECDsaEncryptorDescriptor>>>()));
        }

        private static void AddHttpAdminStore(IServiceCollection services,
            Type entityType,
            Func<IServiceProvider, Task<HttpClient>> getHttpClient)
        {
            var iAdminStoreType = typeof(IAdminStore<>)
                                .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();

            services.AddTransient(iAdminStoreType, provider =>
            {
                return CreateStore(getHttpClient, provider, entityType);
            });
        }

        private static IEnumerable<Type> GetEntityTypes()
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entityTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                t.GetInterface("IEntityId") != null);
            return entityTypeList;
        }

        private static object CreateStore(Func<IServiceProvider, Task<HttpClient>> getHttpClient, IServiceProvider provider, Type entityType)
        {
            var adminStoreType = typeof(AdminStore<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();

            var loggerType = typeof(ILogger<>).MakeGenericType(adminStoreType);
            return adminStoreType.GetConstructors()[0]
                .Invoke(new object[] { getHttpClient.Invoke(provider), provider.GetRequiredService(loggerType) });
        }
    }
}
