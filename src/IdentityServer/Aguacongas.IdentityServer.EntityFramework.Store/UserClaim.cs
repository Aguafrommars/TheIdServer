// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class UserClaim : IdentityUserClaim<string>
    {
        /// <summary>
        /// Gets or sets the issuer.
        /// </summary>
        /// <value>
        /// The issuer.
        /// </value>
        public virtual string Issuer { get; set; }

        /// <summary>
        /// Gets or sets the original value.
        /// </summary>
        /// <value>
        /// The original value.
        /// </value>
        public virtual string OriginalType { get; set; }

        /// <summary>
        /// Converts to claim.
        /// </summary>
        /// <returns></returns>
        public override Claim ToClaim()
        {
            var claim = new Claim(ClaimType, ClaimValue, null, Issuer);
            claim.Properties.Add(nameof(OriginalType), OriginalType);
            return claim;
        }

        /// <summary>
        /// Reads the type and value from the Claim.
        /// </summary>
        /// <param name="claim"></param>
        public override void InitializeFromClaim(Claim claim)
        {
            base.InitializeFromClaim(claim);
            Issuer = claim.Issuer;
            if (claim.Properties.TryGetValue(nameof(OriginalType), out string originalType))
            {
                OriginalType = originalType;
            }
        }
    }
}
