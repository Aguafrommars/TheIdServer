using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Aguacongas.IdentityServer.Store
{
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IAdminStore<Entity.BackChannelAuthenticationRequest> _backChannelAuthenticationRequestStore;
        private readonly IAdminStore<Entity.AuthorizationCode> _authorizationCodeStore;
        private readonly IAdminStore<ReferenceToken> _referenceTokenStore;
        private readonly IAdminStore<Entity.RefreshToken> _refreshTokenStore;
        private readonly IAdminStore<UserConsent> _userConsentStore;

        public PersistedGrantStore(
            IAdminStore<Entity.BackChannelAuthenticationRequest> backChannelAuthenticationRequestStore,
            IAdminStore<Entity.AuthorizationCode> authorizationCodeStore,
            IAdminStore<ReferenceToken> referenceTokenStore,
            IAdminStore<Entity.RefreshToken> refreshToken,
            IAdminStore<UserConsent> userConsentStore)
        {
            _backChannelAuthenticationRequestStore = backChannelAuthenticationRequestStore ?? throw new ArgumentNullException(nameof(backChannelAuthenticationRequestStore));
            _authorizationCodeStore = authorizationCodeStore ?? throw new ArgumentNullException(nameof(authorizationCodeStore));
            _referenceTokenStore = referenceTokenStore ?? throw new ArgumentNullException(nameof(referenceTokenStore));
            _refreshTokenStore = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken));
            _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
        }

        public Task<IReadOnlyCollection<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<PersistedGrant> GetAsync(string key, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAllAsync(PersistedGrantFilter filter, CancellationToken ct)
        {
            filter.Validate();

            var grantTypes = FilterGrantTypes(filter);
            var query = GetOdataFilter(filter);

            foreach (var type in grantTypes)
            {
                switch (type)
                {
                    case PersistedGrantTypes.AuthorizationCode:
                        await DeleteAllAsync(query, _authorizationCodeStore, ct).ConfigureAwait(false);
                        break;
                    case PersistedGrantTypes.RefreshToken:
                        await DeleteAllAsync(query, _refreshTokenStore, ct).ConfigureAwait(false);
                        break;
                    case PersistedGrantTypes.ReferenceToken:
                        await DeleteAllAsync(query, _referenceTokenStore, ct).ConfigureAwait(false);
                        break;
                    case PersistedGrantTypes.UserConsent:
                        await DeleteAllAsync(query, _userConsentStore, ct).ConfigureAwait(false);
                        break;
                    case PersistedGrantTypes.BackChannelAuthenticationRequest:
                        await DeleteAllAsync(query, _backChannelAuthenticationRequestStore, ct).ConfigureAwait(false);
                        break;
                    default: throw new InvalidOperationException($"Grant type '{type}' unsupported.");
                }
            }
        }

        public Task RemoveAsync(string key, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task StoreAsync(PersistedGrant grant, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        private static string GetOdataFilter(PersistedGrantFilter filter)
        {
            filter.Validate();

            var filterList = new List<string>();

            if (filter.SessionId is not null)
            {
                filterList.Add($"{nameof(IGrant.SessionId)} eq '{filter.SessionId}'");
            }
            if (filter.SubjectId is not null)
            {
                filterList.Add($"{nameof(IGrant.UserId)} eq '{filter.SubjectId}'");
            }

            var clientId = filter.ClientId;
            if (filter.ClientIds is not null)
            {
                var clientIds = filter.ClientIds.ToList();
                if (clientId is not null)
                {
                    clientIds.Add(clientId);
                    clientId = null;
                }

                var clientIdsFilter = string.Join(" or ", clientIds.Select(id => $"{nameof(IGrant.ClientId)} eq '{id}'"));
                filterList.Add($"({clientIdsFilter})");
            }
            if (clientId is not null)
            {
                filterList.Add($"{nameof(IGrant.ClientId)} eq '{filter.ClientId}'");
            }

            return string.Join(" and ", filterList);
        }

        private static List<string> FilterGrantTypes(PersistedGrantFilter filter)
        {
            var responseList = new List<string>
            {
                PersistedGrantTypes.AuthorizationCode,
                PersistedGrantTypes.ReferenceToken,
                PersistedGrantTypes.RefreshToken,
                PersistedGrantTypes.UserConsent,
                PersistedGrantTypes.BackChannelAuthenticationRequest
            };

            var type = filter.Type;
            if (filter.Types is not null)
            {
                var types = filter.Types.ToList();
                if (filter.Type is not null)
                {
                    types.Add(filter.Type);
                    type = null;
                }
                responseList = responseList.Where(gt => types.Contains(gt)).ToList();
            }

            if (type is not null)
            {
                responseList = responseList.Where(gt => gt == type).ToList();
            }

            return responseList;
        }

        private static async Task DeleteAllAsync<T>(string query, IAdminStore<T> store, CancellationToken ct) where T : class, IEntityId
        {
            var response = await store.GetAsync(new PageRequest
            {
                Filter = query,
                Select = nameof(IEntityId.Id)
            }, ct).ConfigureAwait(false);
            foreach (var id in response.Items.Select(e => e.Id))
            {
                await store.DeleteAsync(id, ct);
            }
        }
    }
}
