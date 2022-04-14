using Aguacongas.IdentityServer.Store.Entity;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public class ServerSideSessionStore : IServerSideSessionStore
    {
        private readonly IAdminStore<UserSession> _store;

        public ServerSideSessionStore(IAdminStore<UserSession> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public Task CreateSessionAsync(ServerSideSession session, CancellationToken cancellationToken = default)
        => _store.CreateAsync(session.ToUserSession(), cancellationToken);

        public Task DeleteSessionAsync(string key, CancellationToken cancellationToken = default)
        => _store.DeleteAsync(key, cancellationToken);

        public async Task DeleteSessionsAsync(SessionFilter filter, CancellationToken cancellationToken = default)
        {
            const int memberCount = 2;
            var expressionList = new List<string>(memberCount);
            if (!string.IsNullOrEmpty(filter.SubjectId))
            {
                expressionList.Add($"{nameof(UserSession.UserId)} eq '{filter.SubjectId}'");
            }            
            if (!string.IsNullOrEmpty(filter.SessionId))
            {
                expressionList.Add($"{nameof(UserSession.SessionId)} eq '{filter.SessionId}'");
            }
            
            var pageResponse = await _store.GetAsync(new PageRequest
            {
                Filter = string.Join(" and ", expressionList),
                Select = nameof(UserSession.Id),
            }, cancellationToken).ConfigureAwait(false);

            foreach(var sessionId in pageResponse.Items.Select(u => u.Id))
            {
                await _store.DeleteAsync(sessionId, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<IReadOnlyCollection<ServerSideSession>> GetAndRemoveExpiredSessionsAsync(int count, CancellationToken cancellationToken = default)
        {
            var pageResponse = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(UserSession.Expires)} lt {DateTime.UtcNow:o}",
                OrderBy = nameof(UserSession.Created),
                Take = count    
            }, cancellationToken).ConfigureAwait(false);

            var items = pageResponse.Items;
            foreach (var sessionId in items.Select(s => s.Id))
            {
                await _store.DeleteAsync(sessionId, cancellationToken).ConfigureAwait(false);
            }

            return items.Select(s => s.ToServerSideSession()).ToArray();
        }

        public async Task<ServerSideSession> GetSessionAsync(string key, CancellationToken cancellationToken = default)
        => (await _store.GetAsync(key, new GetRequest(), cancellationToken).ConfigureAwait(false))?.ToServerSideSession();

        public async Task<IReadOnlyCollection<ServerSideSession>> GetSessionsAsync(SessionFilter filter, CancellationToken cancellationToken = default)
        {
            const int memberCount = 2;
            var expressionList = new List<string>(memberCount);
            if (!string.IsNullOrEmpty(filter.SubjectId))
            {
                expressionList.Add($"{nameof(UserSession.UserId)} eq '{filter.SubjectId}'");
            }
            if (!string.IsNullOrEmpty(filter.SessionId))
            {
                expressionList.Add($"{nameof(UserSession.SessionId)} eq '{filter.SessionId}'");
            }

            var pageResponse = await _store.GetAsync(new PageRequest
            {
                Filter = string.Join(" and ", expressionList),
            }, cancellationToken).ConfigureAwait(false);

            return pageResponse.Items.Select(s => s.ToServerSideSession()).ToArray();
        }

        public async  Task<QueryResult<ServerSideSession>> QuerySessionsAsync(SessionQuery filter = null, CancellationToken cancellationToken = default)
        {
            filter ??= new();

            const int memberCount = 3;
            var expressionList = new List<string>(memberCount);
            if (!string.IsNullOrEmpty(filter.SubjectId))
            {
                expressionList.Add($"{nameof(UserSession.UserId)} eq '{filter.SubjectId}'");
            }
            if (!string.IsNullOrEmpty(filter.SessionId))
            {
                expressionList.Add($"{nameof(UserSession.SessionId)} eq '{filter.SessionId}'");
            }
            if (!string.IsNullOrEmpty(filter.DisplayName))
            {
                expressionList.Add($"containt(tolower({nameof(UserSession.DisplayName)}), '{filter.DisplayName.ToLowerInvariant()}')");
            }
            var odataFilter = string.Join(" or ", expressionList);

            const int takeDefault = 25;
            var take = filter.CountRequested == 0 ? takeDefault : filter.CountRequested;
            var initialSkip = filter.ResultsToken == null ? 0 : int.Parse(filter.ResultsToken);
            var skip = filter.RequestPriorResults ? initialSkip - take : initialSkip;
            skip = skip < 0 ? 0 : skip;
            var pageResponse = await _store.GetAsync(new PageRequest
            {
                Filter = string.IsNullOrEmpty(odataFilter) ? null : odataFilter,
                Take = take,
                Skip = skip
            }, cancellationToken).ConfigureAwait(false);

            var items = pageResponse.Items;
            var skipNext = initialSkip + items.Count();
            var totalPage = pageResponse.Count == 0 ? null : 1 + (int?)Math.Floor((double)pageResponse.Count / take);
            var currentPage = pageResponse.Count == 0 ? null : (int?)Math.Floor((double)(initialSkip + take) / take);
            return new()
            {
                CurrentPage = currentPage,
                HasNextResults = currentPage.HasValue && currentPage != totalPage,
                HasPrevResults = currentPage.HasValue && currentPage > 1,
                Results = pageResponse.Items.Select(s => s.ToServerSideSession()).ToArray(),
                ResultsToken = skipNext.ToString(),
                TotalCount = pageResponse.Count,
                TotalPages = totalPage
            };
        }

        public Task UpdateSessionAsync(ServerSideSession session, CancellationToken cancellationToken = default)
        => _store.UpdateAsync(session.ToUserSession(), cancellationToken);
    }
}
