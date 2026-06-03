// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores.Serialization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.IdentityServer.Admin.Services;

/// <summary>
/// <see cref="IPersistedGrantService"/> implementation
/// </summary>
/// <seealso cref="IPersistedGrantService" />
/// <remarks>
/// Initializes a new instance of the <see cref="PersistedGrantService"/> class.
/// </remarks>
/// <param name="authorizationCodeStore">The authorization code store.</param>
/// <param name="userConsentStore">The user consent store.</param>
/// <param name="refreshTokenStore">The refresh token store.</param>
/// <param name="referenceTokenStore">The reference token store.</param>
/// <param name="serializer">The serializer.</param>
/// <param name="localizer">The string localizer.</param>
/// <param name="logger">The logger.</param>
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
/// or 
/// localizer
/// or
/// logger
/// </exception>
public class PersistedGrantService(IAdminStore<Entity.AuthorizationCode> authorizationCodeStore,
    IAdminStore<Entity.UserConsent> userConsentStore,
    IAdminStore<Entity.RefreshToken> refreshTokenStore,
    IAdminStore<Entity.ReferenceToken> referenceTokenStore,
    IPersistentGrantSerializer serializer,
    IStringLocalizer<PersistedGrantService> localizer,
    ILogger<PersistedGrantService> logger) : IPersistedGrantService
{
    private readonly IAdminStore<Entity.AuthorizationCode> _authorizationCodeStore = authorizationCodeStore ?? throw new ArgumentNullException(nameof(authorizationCodeStore));
    private readonly IAdminStore<Entity.UserConsent> _userConsentStore = userConsentStore ?? throw new ArgumentNullException(nameof(userConsentStore));
    private readonly IAdminStore<Entity.RefreshToken> _refreshTokenStore = refreshTokenStore ?? throw new ArgumentNullException(nameof(refreshTokenStore));
    private readonly IAdminStore<Entity.ReferenceToken> _referenceTokenStore = referenceTokenStore ?? throw new ArgumentNullException(nameof(referenceTokenStore));
    private readonly IPersistentGrantSerializer _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
    private readonly IStringLocalizer<PersistedGrantService> _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
    private readonly ILogger<PersistedGrantService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <summary>
    /// Gets all grants for a given subject ID.
    /// </summary>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IReadOnlyCollection<Grant>> GetAllGrantsAsync(string subjectId, CancellationToken ct)
    {
        var request = new PageRequest
        {
            Filter = $"{nameof(Entity.IGrant.UserId)} eq '{subjectId}'"
        };

        var consentList = (await _userConsentStore.GetAsync(request, ct).ConfigureAwait(false)).Items
            .Select(c => TryDeserialize<Consent>(c, c => new Grant
            {
                ClientId = c.ClientId,
                CreationTime = c.CreationTime,
                Expiration = c.Expiration,
                Scopes = c.Scopes,
                SubjectId = subjectId
            }));

        var codeList = (await _authorizationCodeStore.GetAsync(request, ct).ConfigureAwait(false)).Items
            .Select(c => TryDeserialize<AuthorizationCode>(c, c => new Grant
            {
                ClientId = c.ClientId,
                CreationTime = c.CreationTime,
                Description = c.Description,
                Expiration = c.CreationTime.AddSeconds(c.Lifetime),
                Scopes = c.RequestedScopes,
                SubjectId = subjectId
            }));

        var refreshTokenList = (await _refreshTokenStore.GetAsync(request, ct).ConfigureAwait(false)).Items
           .Select(t => TryDeserialize<RefreshToken>(t, t => new Grant
           {
               ClientId = t.ClientId,
               CreationTime = t.CreationTime,
               Description = t.Description,
               Expiration = t.CreationTime.AddSeconds(t.Lifetime),
               Scopes = t.AuthorizedScopes,
               SubjectId = subjectId
           }));

        var referenceTokenList = (await _referenceTokenStore.GetAsync(request, ct).ConfigureAwait(false)).Items
           .Select(t => TryDeserialize<Token>(t, t => new Grant
           {
               ClientId = t.ClientId,
               CreationTime = t.CreationTime,
               Description = t.Description,
               Expiration = t.CreationTime.AddSeconds(t.Lifetime),
               Scopes = t.Scopes,
               SubjectId = subjectId
           }));

        consentList = Join(consentList, codeList);
        consentList = Join(consentList, refreshTokenList);
        consentList = Join(consentList, referenceTokenList);

        return [.. consentList];
    }


    /// <summary>
    /// Removes all grants for a given subject id and client id combination.
    /// </summary>
    /// <param name="subjectId">The subject identifier.</param>
    /// <param name="ct"></param>
    /// <param name="clientId">The client identifier.</param>
    /// <param name="sessionId">The sesion id (optional).</param>
    /// <returns></returns>
    public async Task RemoveAllGrantsAsync(string subjectId, CancellationToken ct, string clientId = null, string sessionId = null)
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
            Filter = filter
        };
        var consentListResponse = await _userConsentStore.GetAsync(request, ct).ConfigureAwait(false);
        var codeListResponse = await _authorizationCodeStore.GetAsync(request, ct).ConfigureAwait(false);
        var refreshTokenListResponse = await _refreshTokenStore.GetAsync(request, ct).ConfigureAwait(false);
        var referenceTokenListResponse = await _referenceTokenStore.GetAsync(request, ct).ConfigureAwait(false);

        foreach (var consent in consentListResponse.Items)
        {
            await _userConsentStore.DeleteAsync(consent.Id, ct).ConfigureAwait(false);
        }
        foreach (var code in codeListResponse.Items)
        {
            await _authorizationCodeStore.DeleteAsync(code.Id, ct).ConfigureAwait(false);
        }
        foreach (var token in refreshTokenListResponse.Items)
        {
            await _refreshTokenStore.DeleteAsync(token.Id, ct).ConfigureAwait(false);
        }
        foreach (var token in referenceTokenListResponse.Items)
        {
            await _referenceTokenStore.DeleteAsync(token.Id, ct).ConfigureAwait(false);
        }
    }


    private static List<Grant> Join(IEnumerable<Grant> first, IEnumerable<Grant> second)
    {
        var list = first.ToList();

        foreach (var other in second)
        {
            var match = list.FirstOrDefault(x => x.ClientId == other.ClientId);
            if (match != null)
            {
                match.Scopes = (match.Scopes ?? []).Union(other.Scopes ?? []).Distinct();

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

    private Grant TryDeserialize<TToken>(Entity.IGrant grant, Func<TToken, Grant> createGrant)
    {
        try
        {
            return createGrant(_serializer.Deserialize<TToken>(grant.Data));
        }
        catch (CryptographicException ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            return new Grant
            {
                ClientId = grant.ClientId,
                CreationTime = grant.CreatedAt,
                Expiration = grant.Expiration,
                Description = _localizer["Cannot get the description of an internal error."],
                SubjectId = grant.UserId,
                Scopes = []
            };
        }
    }
}
