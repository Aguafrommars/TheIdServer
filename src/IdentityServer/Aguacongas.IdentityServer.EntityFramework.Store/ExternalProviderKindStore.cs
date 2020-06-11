using Aguacongas.AspNetCore.Authentication;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
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
            var count = _manager.ManagedHandlerType.Count();
            var typeList = _manager.ManagedHandlerType;
            if (request.Take.HasValue)
            {
                typeList = typeList
                    .Skip(request.Skip ?? 0)
                    .Take(request.Take.Value);
            }
            return Task.FromResult(new PageResponse<ExternalProviderKind>
            {
                Count = count,
                Items = typeList
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
