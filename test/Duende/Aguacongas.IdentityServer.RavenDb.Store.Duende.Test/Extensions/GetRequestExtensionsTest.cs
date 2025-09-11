// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace Aguacongas.IdentityServer.RavenDb.Store.Test.Extensions
{
    public class GetRequestExtensionsTest
    {
        [Fact]
        public void ToWhereClause_should_parse_filter_expression()
        {
            var sut = new PageRequest
            {
                Filter = "Id eq 'test' and contains(Email, 'test')"
            };

            var actual = sut.ToWhereClause<IdentityUser, string>(i => i.Id);

            Assert.Equal("where Id = 'test' and search(Email,'*test*')", actual);
        }
    }
}
