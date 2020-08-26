// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// <see cref="IPersistedGrantService"/> implementation
    /// </summary>
    /// <seealso cref="IPersistedGrantService" />
    public class PersistedGrantService : IPersistedGrantService
    {
        private readonly IAdminStore<Entity.AuthorizationCode> _authorizationCodeStore;
        private readonly IAdminStore<Entity.UserConsent> _userConsentStore;
        private readonly IAdminStore<Entity.RefreshToken> _refreshTokenStore;
        private readonly IAdminStore<Entity.ReferenceToken> _referenceTokenStore;
        private readonly IPersistentGrantSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantService"/> class.
        /// </summary>
        /// <param name="authorizationCodeStore">The authorization code store.</param>
        /// <param name="userConsentStore">The user consent store.</param>
        /// <param name="refreshTokenStore">The refresh token store.</param>
        /// <param name="referenceTokenStore">The reference token store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <exception cref="ArgumentNullException">
        /// authorizationCodeStore
        /// or
        /// userConsentStore
        /// or
        /// refreshTokenStore
        /// or
        /// referenceTokenStore
        /// or
        /// serializer
        /// </exception>
        public PersistedGrantService(IAdminStore<Entity.AuthorizationCode> authorizationCodeStore,
            IAdminStore<Entity.UserConsent> userConsentStore,
            IAdminStore<Entity.RefreshToken> refreshTokenStore,
            IAdminStore<Entity.ReferenceToken> referenceTokenStore,
            IPersistentGrantSerializer serializer)
        {
            _authorizationCodeStore = authorizationCodeStore ?? throw new ArgumentNullException(nameof(authorizationCodeStore));
            _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
            _refreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
            _referenceTokenStore = referenceTokenStore ?? throw new ArgumentNullException(nameof(referenceTokenStore));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        /// <summary>
        /// Gets all grants for a given subject ID.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <returns></returns>
        public async Task<IEnumerable<Grant>> GetAllGrantsAsync(string subjectId)
        {
            var request = new PageRequest
            {
                Take = null,
                Filter = $"{nameof(Entity.IGrant.UserId)} eq '{subjectId}'"
            };

            var consentList = (await _userConsentStore.GetAsync(request).ConfigureAwait(false)).Items
                .Select(c => _serializer.Deserialize<Consent>(c.Data))
                .Select(c => new Grant
                {
                    ClientId = c.ClientId,
                    CreationTime = c.CreationTime,
                    Expiration = c.Expiration,
                    Scopes = c.Scopes,
                    SubjectId = subjectId
                });

            var codeList = (await _authorizationCodeStore.GetAsync(request).ConfigureAwait(false)).Items
                .Select(c => _serializer.Deserialize<AuthorizationCode>(c.Data))
                .Select(c => new Grant
                {
                    ClientId = c.ClientId,
                    CreationTime = c.CreationTime,
                    Description = c.Description,
                    Expiration = c.CreationTime.AddSeconds(c.Lifetime),
                    Scopes = c.RequestedScopes,
                    SubjectId = subjectId
                });

            var refreshTokenList = (await _refreshTokenStore.GetAsync(request).ConfigureAwait(false)).Items
               .Select(t => _serializer.Deserialize<RefreshToken>(t.Data))
               .Select(t => new Grant
               {
                   ClientId = t.ClientId,
                   CreationTime = t.CreationTime,
                   Description = t.Description,
                   Expiration = t.CreationTime.AddSeconds(t.Lifetime),
                   Scopes = t.Scopes,
                   SubjectId = subjectId
               });

            var referenceTokenList = (await _referenceTokenStore.GetAsync(request).ConfigureAwait(false)).Items
               .Select(t => _serializer.Deserialize<Token>(t.Data))
               .Select(t => new Grant
               {
                   ClientId = t.ClientId,
                   CreationTime = t.CreationTime,
                   Description = t.Description,
                   Expiration = t.CreationTime.AddSeconds(t.Lifetime),
                   Scopes = t.Scopes,
                   SubjectId = subjectId
               });

            consentList = Join(consentList, codeList);
            consentList = Join(consentList, refreshTokenList);
            consentList = Join(consentList, referenceTokenList);

            return consentList;
        }
            

        /// <summary>
        /// Removes all grants for a given subject id and client id combination.
        /// </summary>
        /// <param name="subjectId">The subject identifier.</param>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="sessionId">The sesion id (optional).</param>
        /// <returns></returns>
        public async Task RemoveAllGrantsAsync(string subjectId, string clientId = null, string sessionId = null)
        {
            var filter = $"{nameof(Entity.IGrant.UserId)} eq '{subjectId}'";
            if (clientId != null)
            {
                filter += $" and {nameof(Entity.IGrant.ClientId)} eq '{clientId}'";
            }
            if (sessionId != null)
            {
                filter += $" and {nameof(Entity.IGrant.SessionId)} eq '{sessionId}'";
            }
            var request = new PageRequest
            {
                Take = null,
                Filter = filter
            };
            var consentListResponse = await _userConsentStore.GetAsync(request).ConfigureAwait(false);
            var codeListResponse = await _authorizationCodeStore.GetAsync(request).ConfigureAwait(false);
            var refreshTokenListResponse = await _refreshTokenStore.GetAsync(request).ConfigureAwait(false);
            var referenceTokenListResponse = await _referenceTokenStore.GetAsync(request).ConfigureAwait(false);

            foreach(var consent in consentListResponse.Items)
            {
                await _userConsentStore.DeleteAsync(consent.Id).ConfigureAwait(false);
            }
            foreach (var code in codeListResponse.Items)
            {
                await _authorizationCodeStore.DeleteAsync(code.Id).ConfigureAwait(false);
            }
            foreach (var token in refreshTokenListResponse.Items)
            {
                await _refreshTokenStore.DeleteAsync(token.Id).ConfigureAwait(false);
            }
            foreach (var token in referenceTokenListResponse.Items)
            {
                await _referenceTokenStore.DeleteAsync(token.Id).ConfigureAwait(false);
            }
        }
            

        private IEnumerable<Grant> Join(IEnumerable<Grant> first, IEnumerable<Grant> second)
        {
            var list = first.ToList();

            foreach (var other in second)
            {
                var match = list.FirstOrDefault(x => x.ClientId == other.ClientId);
                if (match != null)
                {
                    match.Scopes = match.Scopes.Union(other.Scopes).Distinct();

                    if (match.CreationTime > other.CreationTime)
                    {
                        // show the earlier creation time
                        match.CreationTime = other.CreationTime;
                    }

                    if (match.Expiration == null || other.Expiration == null)
                    {
                        // show that there is no expiration to one of the grants
                        match.Expiration = null;
                    }
                    else if (match.Expiration < other.Expiration)
                    {
                        // show the latest expiration
                        match.Expiration = other.Expiration;
                    }

                    match.Description ??= other.Description;
                }
                else
                {
                    list.Add(other);
                }
            }

            return list;
        }
    }
}
