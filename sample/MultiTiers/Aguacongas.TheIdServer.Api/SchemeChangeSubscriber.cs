using Aguacongas.IdentityServer.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Api;

public class SchemeChangeSubscriber : ISchemeChangeSubscriber
{
    public Task SubscribeAsync(CancellationToken cancellationToken)
    => Task.CompletedTask;

    public Task UnSubscribeAsync(CancellationToken cancellationToken)
    => Task.CompletedTask;
}
