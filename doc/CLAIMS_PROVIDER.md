# Claims providers

You can add your customs claims providers to the server. The [`ProfileService`](https://github.com/Aguafrommars/TheIdServer/blob/master/src/IdentityServer/Aguacongas.IdentityServer.Admin/Services/ProfileService.cs) load claims providers defined in resources properties.

When a client ask for a resource (Identity or API), the [`ProfileService`](https://github.com/Aguafrommars/TheIdServer/blob/master/src/IdentityServer/Aguacongas.IdentityServer.Admin/Services/ProfileService.cs) looks for **ClaimProviderType** key in its properties. If this key is found, it create an instance of the corresponding assembly qualified type name and call the method  [`ProvideClaims`](https://github.com/Aguafrommars/TheIdServer/blob/master/src/IdentityServer/Aguacongas.IdentityServer/Abstractions/IProvideClaims.cs) with the current context subject, client, caller and properties.

Additionaly, it can load the class from assembly path defined in **ClaimProviderAssemblyPath** resource's property.

## Configruation

In a resource (Identity or API) your client ask for, add the property **ClaimProviderType** with the assembly qualified name of a class implementing [`IProvideClaims`](https://github.com/Aguafrommars/TheIdServer/blob/master/src/IdentityServer/Aguacongas.IdentityServer/Abstractions/IProvideClaims.cs) interface.

Add the path to the assembly containing this class in the property **ClaimProviderAssemblyPath**.

![claims-provider](assets/claims-provider-configuration.png)

## Implement

Claims providers must implements [`IProvideClaims`](https://github.com/Aguafrommars/TheIdServer/blob/master/src/IdentityServer/Aguacongas.IdentityServer/Abstractions/IProvideClaims.cs) interface.

**sample**

The project [sample/Aguacongas.TheIdServer.CustomClaimsProvider](sample/Aguacongas.TheIdServer.CustomClaimsProvider) contains an implementation sample.

```cs
public class MapClaimsProvider: IProvideClaims
{
    public Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource)
    {
        var defaultOutboundClaimMap = JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap;
        var claims = new List<Claim>(subject.Claims.Count());
        foreach (var claim in subject.Claims)
        {
            if (defaultOutboundClaimMap.TryGetValue(claim.Type, out string toClaimType))
            {
                claims.Add(new Claim(toClaimType, claim.Value, claim.ValueType, claim.Issuer));
            }
        }

        return Task.FromResult(claims as IEnumerable<Claim>);
    }
}
```