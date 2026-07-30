using Duende.IdentityServer.Events;
using Duende.IdentityServer.Services;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Api;

public class EmptyEventService : IEventService
{
    public bool CanRaiseEventType(EventTypes evtType)
    => true;

    public Task RaiseAsync(Event evt, CancellationToken ct)
    => Task.CompletedTask;
}
