// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation.Stores
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRelyingPartyStore
    {
        /// <summary>
        /// Finds the relying party by realm.
        /// </summary>
        /// <param name="realm">The realm.</param>
        /// <returns></returns>
        Task<RelyingParty> FindRelyingPartyByRealm(string realm);
    }
}
