using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IProviderClient
    {
        Task ProviderAdded(string scheme, CancellationToken cancellationToken = default);

        Task ProviderUpdated(string scheme, CancellationToken cancellationToken = default);

        Task ProviderRemoved(string scheme, CancellationToken cancellationToken = default);
    }
}
