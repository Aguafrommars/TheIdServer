// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore = Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class UserStore<TUser, TRole, TContext>
        : EntityFrameworkCore.UserStore<TUser, TRole, TContext, string, UserClaim, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
        where TContext : DbContext
    {
        private DbSet<UserClaim> UserClaims { get { return Context.Set<UserClaim>(); } }

        public UserStore(TContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
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
            foreach (var claim in claims)
            {
                var matchedClaims = await UserClaims.Where(uc => uc.UserId.Equals(user.Id) &&
                    uc.Issuer == claim.Issuer &&
                    uc.ClaimValue == claim.Value &&
                    uc.ClaimType == claim.Type)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var c in matchedClaims)
                {
                    UserClaims.Remove(c);
                }
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
