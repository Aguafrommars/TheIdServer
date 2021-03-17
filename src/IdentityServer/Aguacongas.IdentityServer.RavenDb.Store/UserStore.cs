// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.Identity.RavenDb;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.RavenDb.Store
{
    public class UserStore<TUser, TRole>
        : UserStore<TUser, string, TRole, UserClaim, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
    {
        private readonly IAsyncDocumentSession _session;
        public UserStore(ScopedAsynDocumentcSession session, UserOnlyStore<TUser, string, UserClaim, IdentityUserLogin<string>, IdentityUserToken<string>> userOnlyStore, IdentityErrorDescriber describer = null) 
            : base(session.Session, userOnlyStore, describer)
        {
            _session = session.Session;
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public override async Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken = default)
        {
            CheckParameters(user, claims);

            var userId = ConvertIdToString(user.Id);
            var data = await _session.LoadAsync<UserData>($"userdata/{userId}", cancellationToken).ConfigureAwait(false);

            var toDeleteList = new List<string>();
            foreach (var claimId in data.ClaimIds)
            {
                var userClaim = await _session.LoadAsync<UserClaim>(claimId, cancellationToken).ConfigureAwait(false);
                if (claims.Any(c => userClaim.Issuer == c.Issuer && userClaim.ClaimType == c.Type && userClaim.ClaimValue == c.Value))
                {
                    toDeleteList.Add(claimId);
                }
            }

            foreach (var claimId in toDeleteList)
            {
                _session.Delete(claimId);
                data.ClaimIds.Remove(claimId);
            }
        }

        private void CheckParameters(TUser user, IEnumerable<Claim> claims)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (claims == null)
            {
                throw new ArgumentNullException(nameof(claims));
            }
        }
    }
}
