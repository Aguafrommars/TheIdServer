// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace System
{
    public static class ServiceProviderExtensions
    {
        public static object CreateRoleStore(this IServiceProvider provider, Type roleStoreType)
        {
            roleStoreType = roleStoreType ?? throw new ArgumentNullException(nameof(roleStoreType));
            var RoleClaimeStore = provider.GetRequiredService<IAdminStore<RoleClaim>>();
            var roleStore = provider.GetRequiredService<IAdminStore<Role>>();
            var errorDescriber = provider.GetRequiredService<IdentityErrorDescriber>();
            var constructor = roleStoreType.GetConstructor(new Type[]
            {
                typeof(IAdminStore<Role>),
                typeof(IAdminStore<RoleClaim>),
                typeof(IdentityErrorDescriber)
            });
            return constructor.Invoke(new object[]
            {
                roleStore,
                RoleClaimeStore,
                errorDescriber
            });
        }

        public static object CreateUserStore(this IServiceProvider provider, Type userOnlyStoreType, Type userStoreType)
        {
            userOnlyStoreType = userOnlyStoreType ?? throw new ArgumentNullException(nameof(userOnlyStoreType));
            userStoreType = userStoreType ?? throw new ArgumentNullException(nameof(userStoreType));
            var userStore = provider.GetRequiredService<IAdminStore<Role>>();
            var userRoleStore = provider.GetRequiredService<IAdminStore<UserRole>>();
            var userOnlyStore = provider.GetRequiredService(userOnlyStoreType);
            var errorDescriber = provider.GetRequiredService<IdentityErrorDescriber>();
            var constructor = userStoreType.GetConstructor(new Type[]
            {
                typeof(IAdminStore<Role>),
                typeof(IAdminStore<UserRole>),
                userOnlyStoreType,
                typeof(IdentityErrorDescriber)
            });
            return constructor.Invoke(new object[]
            {
                userStore,
                userRoleStore,
                userOnlyStore,
                errorDescriber
            });
        }

        public static object CreateUserOnlyStore(this IServiceProvider provider, Type userOnlyStoreType)
        {
            userOnlyStoreType = userOnlyStoreType ?? throw new ArgumentNullException(nameof(userOnlyStoreType));
            var userStore = provider.GetRequiredService<IAdminStore<User>>();
            var userClaimStore = provider.GetRequiredService<IAdminStore<UserClaim>>();
            var userLoginStore = provider.GetRequiredService<IAdminStore<UserLogin>>();
            var userTokenStore = provider.GetRequiredService<IAdminStore<UserToken>>();
            var errorDescriber = provider.GetRequiredService<IdentityErrorDescriber>();
            var constructor = userOnlyStoreType.GetConstructor(new Type[]
            {
                typeof(IAdminStore<User>),
                typeof(IAdminStore<UserClaim>),
                typeof(IAdminStore<UserLogin>),
                typeof(IAdminStore<UserToken>),
                typeof(IdentityErrorDescriber)
            });
            return constructor.Invoke(new object[]
            {
                userStore,
                userClaimStore,
                userLoginStore,
                userTokenStore,
                errorDescriber,
            });
        }
    }
}
