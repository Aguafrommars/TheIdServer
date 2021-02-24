// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.Identity.RavenDb;
using Microsoft.AspNetCore.Identity;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
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
        public UserStore(IAsyncDocumentSession session, UserOnlyStore<TUser, string, UserClaim, IdentityUserLogin<string>, IdentityUserToken<string>> userOnlyStore, IdentityErrorDescriber describer = null) 
            : base(session, userOnlyStore, describer)
        {
            _session = session;
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
            var data = await _session.LoadAsync<UserData<string, TUser, UserClaim, IdentityUserLogin<string>>>($"user/{userId}", cancellationToken).ConfigureAwait(false);

            var claimList = data.Claims;

            foreach (var claim in claims)
            {
                claimList.RemoveAll(uc => uc.UserId.Equals(user.Id) &&
                    uc.Issuer == claim.Issuer && 
                    uc.ClaimType == claim.Type && 
                    uc.ClaimValue == claim.Value);
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
