using Duende.IdentityServer.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Api;

public class EmptyClientConfigurationValidator : IClientConfigurationValidator
{
    public Task ValidateAsync(ClientConfigurationValidationContext context, CancellationToken ct)
    => Task.CompletedTask;
}
