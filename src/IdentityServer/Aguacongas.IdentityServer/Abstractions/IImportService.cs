using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Abstractions
{
    public interface IImportService
    {
        Task<ImportResult> ImportAsync(IEnumerable<IFormFile> files);
    }
}