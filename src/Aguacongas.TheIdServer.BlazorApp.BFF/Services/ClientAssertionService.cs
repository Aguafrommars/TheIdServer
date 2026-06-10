using Duende.AccessTokenManagement;
using Duende.IdentityModel;
using Duende.IdentityModel.Client;

namespace Aguacongas.TheIdServer.BlazorApp.BFF.Services;

public class ClientAssertionService(AssertionService assertionService) : IClientAssertionService
{
    public Task<ClientAssertion?> GetClientAssertionAsync(ClientCredentialsClientName? clientName = null, TokenRequestParameters? parameters = null, CancellationToken ct = default)
    => Task.FromResult<ClientAssertion?>(new ClientAssertion
    {
        Type = OidcConstants.ClientAssertionTypes.JwtBearer,
        Value = assertionService.CreateClientToken()
    });
}
