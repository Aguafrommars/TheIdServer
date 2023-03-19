// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.WsFederation.Stores
{
    /// <summary>
    /// IRelyingPartyStore implementation
    /// </summary>
    /// <seealso cref="IRelyingPartyStore" />
    public class RelyingPartyStore : IRelyingPartyStore
    {
        private readonly IAdminStore<Entity.Client> _clientStore;
        private readonly IAdminStore<Entity.RelyingParty> _relyingPartyStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelyingPartyStore" /> class.
        /// </summary>
        /// <param name="clientStore">The client store.</param>
        /// <param name="relyingPartyStore">The relying party store.</param>
        /// <exception cref="ArgumentNullException">adminStore</exception>
        public RelyingPartyStore(IAdminStore<Entity.Client> clientStore, IAdminStore<Entity.RelyingParty> relyingPartyStore)
        {
            _clientStore = clientStore ?? throw new ArgumentNullException(nameof(clientStore));
            _relyingPartyStore = relyingPartyStore ?? throw new ArgumentNullException(nameof(relyingPartyStore));
        }

        /// <summary>
        /// Finds the relying party by realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns></returns>
        public async Task<RelyingParty> FindRelyingPartyByRealm(string realm)
        {
            var client = await _clientStore.GetAsync(realm, null).ConfigureAwait(false);
            var relyingPartyId = client.RelyingPartyId;
            var entity = await _relyingPartyStore.GetAsync(relyingPartyId, new GetRequest
            {
                Expand = nameof(Entity.RelyingParty.ClaimMappings)
            }).ConfigureAwait(false);

            if (entity == null)
            {
                return null;
            }

            return new RelyingParty
            {
                ClaimMapping = entity.ClaimMappings.ToDictionary(m => m.FromClaimType, m => m.ToClaimType),
                DigestAlgorithm = entity.DigestAlgorithm,
                EncryptionCertificate = entity.EncryptionCertificate != null ? new X509Certificate2(entity.EncryptionCertificate) : null,
                Realm = entity.Id,
                SamlNameIdentifierFormat = entity.SamlNameIdentifierFormat,
                SignatureAlgorithm = entity.SignatureAlgorithm,
                TokenType = entity.TokenType
            };
        }
    }
}
