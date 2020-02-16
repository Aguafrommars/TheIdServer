// Project: aguacongas/Identity.Firebase
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
    public class UserStoreTest : IdentitySpecificationTestBase<TestUser, TestRole>, IClassFixture<TheIdServerTestFixture>
    {
        private readonly TheIdServerTestFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public UserStoreTest(ITestOutputHelper testOutputHelper, TheIdServerTestFixture fixture)
        {
            _fixture = fixture;
            fixture.TestOutputHelper = testOutputHelper;
            _testOutputHelper = testOutputHelper;
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
            var roleType = typeof(TestRole);
            var userStoreType = typeof(UserStore<,>).MakeGenericType(userType, roleType);
            var userOnlyStoreType = typeof(UserOnlyStore<>).MakeGenericType(userType);
            services.TryAddSingleton(typeof(UserOnlyStore<>).MakeGenericType(userType), 
                p => p.CreateUserOnlyStore(userOnlyStoreType));
            services.TryAddSingleton(typeof(IUserStore<>).MakeGenericType(userType),
                p => p.CreateUserStore(userOnlyStoreType, userStoreType));
            var httpClient = _fixture.Sut.CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "/api");
            services.AddIdentityServer4AdminHttpStores(p =>
            {
                return Task.FromResult(httpClient);
            }).AddLogging(options => options.AddProvider(_fixture.LoggerProvider));
            _fixture.Sut.Services.GetRequiredService<TestUserService>().SetTestUser(true,
                new Claim[]
                {
                    new Claim("role", SharedConstants.WRITER),
                    new Claim("role", SharedConstants.READER)
                });
        }

        protected override void AddRoleStore(IServiceCollection services, object context = null)
        {
            var roleType = typeof(TestRole);
            var roleStoreType = typeof(RoleStore<>).MakeGenericType(roleType);
            services.TryAddSingleton(typeof(IRoleStore<>).MakeGenericType(roleType), 
                p => p.CreateRoleStore(roleStoreType));
            var httpClient = _fixture.Sut.CreateClient();
            httpClient.BaseAddress = new Uri(httpClient.BaseAddress, "/api"); 
            services.AddIdentityServer4AdminHttpStores(p =>
            {
                return Task.FromResult(httpClient);
            }).AddLogging(options => options.AddProvider(_fixture.LoggerProvider));
            _fixture.Sut.Services.GetRequiredService<TestUserService>().SetTestUser(true,
                new Claim[]
                {
                    new Claim("role", SharedConstants.WRITER),
                    new Claim("role", SharedConstants.READER)
                });
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
