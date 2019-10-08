using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public interface IAdminStore<T> where T:class
    {
        IQueryable<T> Items { get; }

        Task<T> CreateAsync(T client, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T client, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, CancellationToken cancellationToken = default);
    }
}
