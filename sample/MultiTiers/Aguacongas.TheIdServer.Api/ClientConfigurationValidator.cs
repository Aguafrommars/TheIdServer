using Duende.IdentityServer.Validation;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Api;

public class EmptyClientConfigurationValidator : IClientConfigurationValidator
{
    public Task ValidateAsync(ClientConfigurationValidationContext context)
    => Task.CompletedTask;
}
