using IdentityServer4.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Store
{
    public interface IGetAllUserConsentStore
    {
        Task<IEnumerable<Consent>> GetAllUserConsent(string subjectId);
    }
}
