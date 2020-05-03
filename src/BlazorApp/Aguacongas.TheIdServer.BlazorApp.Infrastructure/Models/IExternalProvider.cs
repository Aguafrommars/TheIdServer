using Aguacongas.IdentityServer.Store.Entity;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public interface IExternalProvider<TOptions> where TOptions : RemoteAuthenticationOptions
    {
        TOptions DefaultOptions { get; }
        IEnumerable<ExternalProviderKind> Kinds { get; set; }
        TOptions Options { get; set; }
    }
}