using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class ExternalProviderKindStore : IExternalProviderKindStore
    {
        private readonly PersistentDynamicManager<SchemeDefinition> _manager;
        private readonly IAuthenticationSchemeOptionsSerializer _serializer;

        public ExternalProviderKindStore(PersistentDynamicManager<SchemeDefinition> manager, IAuthenticationSchemeOptionsSerializer serializer)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task<PageResponse<ExternalProviderKind>> GetAsync(PageRequest request)
        {
            return Task.FromResult(new PageResponse<ExternalProviderKind>
            {
                Count = _manager.ManagedHandlerType.Count(),
                Items = _manager.ManagedHandlerType
                    .Skip(request.Skip ?? 0)
                    .Take(request.Take)
                    .Select(t => new ExternalProviderKind
                    {
                        KindName = t.GetAuthenticationSchemeOptionsType().Name.Replace("Options", ""),
                        SerializedHandlerType = _serializer.SerializeType(t),
                        SerializedDefaultOptions = _serializer.SerializeOptions(Activator.CreateInstance(t.GetAuthenticationSchemeOptionsType()) as AuthenticationSchemeOptions, t.GetAuthenticationSchemeOptionsType())
                    })
            });
        }
    }
}
