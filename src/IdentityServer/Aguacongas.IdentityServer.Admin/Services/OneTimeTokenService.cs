// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using System;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Gets a one time token and burn it
    /// </summary>
    /// <seealso cref="IRetrieveOneTimeToken" />
    public class OneTimeTokenService : IRetrieveOneTimeToken
    {
        private readonly IAdminStore<OneTimeToken> _store;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneTimeTokenService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public OneTimeTokenService(IAdminStore<OneTimeToken> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Gets the one time token.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public string GetOneTimeToken(string id)
        {
            var token = _store.GetAsync(id, new GetRequest()).GetAwaiter().GetResult();
            if (token == null)
            {
                return null;
            }
            _store.DeleteAsync(id).GetAwaiter().GetResult();
            return token.Data;
        }
    }
}
