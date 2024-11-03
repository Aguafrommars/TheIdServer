using Aguacongas.IdentityServer.Abstractions;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Api;

public class CreatePersonalAccessTokenServvice : ICreatePersonalAccessToken
{
    public Task<string> CreatePersonalAccessTokenAsync(HttpContext context, bool isRefenceToken, int lifetimeDays, IEnumerable<string> apis, IEnumerable<string> scopes, IEnumerable<string> claimTypes)
    {
        throw new System.NotImplementedException();
    }
}
