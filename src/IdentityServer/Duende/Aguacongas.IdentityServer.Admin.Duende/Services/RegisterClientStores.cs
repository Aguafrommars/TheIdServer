using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;

namespace Aguacongas.IdentityServer.Admin.Duende.Services
{
    /// <summary>
    /// Stores used by <see cref="RegisterClientService"/>
    /// </summary>
    public class RegisterClientStores
    {
        /// <summary>
        /// Initialize a new instance of <see cref="RegisterClientStores"/>
        /// </summary>
        /// <param name="clientStore"></param>
        /// <param name="clientUriStore"></param>
        /// <param name="clientResourceStore"></param>
        /// <param name="clientGrantTypeStore"></param>
        /// <param name="clientPropertyStore"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RegisterClientStores(IAdminStore<Client> clientStore,
            IAdminStore<ClientUri> clientUriStore,
            IAdminStore<ClientLocalizedResource> clientResourceStore,
            IAdminStore<ClientGrantType> clientGrantTypeStore,
            IAdminStore<ClientProperty> clientPropertyStore) 
        {
            ClientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            ClientUriStore = clientUriStore ?? throw new ArgumentNullException(nameof(clientUriStore));
            ClientResourceStore = clientResourceStore ?? throw new ArgumentNullException(nameof(clientResourceStore));
            ClientGrantTypeStore = clientGrantTypeStore ?? throw new ArgumentNullException(nameof(clientGrantTypeStore));
            ClientPropertyStore = clientPropertyStore ?? throw new ArgumentNullException(nameof(clientPropertyStore));
        }

        /// <summary>
        /// Gets the client store
        /// </summary>
        public IAdminStore<Client> ClientStore { get; }

        /// <summary>
        /// Gets the client uri stores
        /// </summary>
        public IAdminStore<ClientUri> ClientUriStore { get; }

        /// <summary>
        /// Gets the client resource store
        /// </summary>
        public IAdminStore<ClientLocalizedResource> ClientResourceStore { get; }

        /// <summary>
        /// Gets the client grant type store
        /// </summary>
        public IAdminStore<ClientGrantType> ClientGrantTypeStore { get; }

        /// <summary>
        /// Gets the client property store
        /// </summary>
        public IAdminStore<ClientProperty> ClientPropertyStore { get; }
    }
}
