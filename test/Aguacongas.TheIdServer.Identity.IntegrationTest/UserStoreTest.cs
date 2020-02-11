// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    public class UserStoreTest : IdentitySpecificationTestBase<TestUser, TestRole>, IClassFixture<TheIdServerTestFixture>
    {
        private readonly TheIdServerTestFixture _fixture;

        public UserStoreTest(TheIdServerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CanUpdateUserMultipleTime()
        {
            var manager = CreateManager();
            var user = CreateTestUser();
            IdentityResultAssert.IsSuccess(await manager.CreateAsync(user));
            user = await manager.FindByNameAsync(user.UserName);
            user.PhoneNumber = "+41123456789";
            IdentityResultAssert.IsSuccess(await manager.UpdateAsync(user));
            user.PhoneNumber = "+33123456789";
            IdentityResultAssert.IsSuccess(await manager.UpdateAsync(user));
        }

        protected override void AddUserStore(IServiceCollection services, object context = null)
        {            
            var userType = typeof(TestUser);
            var keyType = typeof(string);
            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, typeof(TestRole));
            var userOnlyStoreType = typeof(UserOnlyStore<>).MakeGenericType(userType);
            services.TryAddSingleton(typeof(UserOnlyStore<>).MakeGenericType(userType), provider => userOnlyStoreType.GetConstructor(new Type[] { typeof(IDatabase), typeof(IdentityErrorDescriber) })
                .Invoke(new object[] { _fixture.Database, null }));
            services.TryAddSingleton(typeof(IUserStore<>).MakeGenericType(userType), provider => userStoreType.GetConstructor(new Type[] { typeof(IDatabase), userOnlyStoreType, typeof(IdentityErrorDescriber) })
                .Invoke(new object[] { _fixture.Database, provider.GetService(userOnlyStoreType), null }));
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var roleType = typeof(TestRole);
            services.TryAddSingleton(typeof(IRoleStore<>).MakeGenericType(roleType), provider => new RoleStore<TestRole>(_fixture.Database));
        }

        protected override object CreateTestContext()
        {
            return null;
        }

        protected override TestUser CreateTestUser(string namePrefix = "", string email = "", string phoneNumber = "",
            bool lockoutEnabled = false, DateTimeOffset? lockoutEnd = default(DateTimeOffset?), bool useNamePrefixAsUserName = false)
        {
            return new TestUser
            {
                UserName = useNamePrefixAsUserName ? namePrefix : string.Format("{0}{1}", namePrefix, Guid.NewGuid()),
                Email = email,
                PhoneNumber = phoneNumber,
                LockoutEnabled = lockoutEnabled,
                LockoutEnd = lockoutEnd
            };
        }

        protected override TestRole CreateTestRole(string roleNamePrefix = "", bool useRoleNamePrefixAsRoleName = false)
        {
            var roleName = useRoleNamePrefixAsRoleName ? roleNamePrefix : string.Format("{0}{1}", roleNamePrefix, Guid.NewGuid());
            return new TestRole(roleName);
        }

        protected override void SetUserPasswordHash(TestUser user, string hashedPassword)
        {
            user.PasswordHash = hashedPassword;
        }

        protected override Expression<Func<TestUser, bool>> UserNameEqualsPredicate(string userName) => u => u.UserName == userName;

        protected override Expression<Func<TestRole, bool>> RoleNameEqualsPredicate(string roleName) => r => r.Name == roleName;

        protected override Expression<Func<TestRole, bool>> RoleNameStartsWithPredicate(string roleName) => r => r.Name.StartsWith(roleName);

        protected override Expression<Func<TestUser, bool>> UserNameStartsWithPredicate(string userName) => u => u.UserName.StartsWith(userName);
    }
}
