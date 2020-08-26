// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class StringLocalizerFactory : IStringLocalizerFactory, ISupportCultures
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly IAdminStore<Culture> _cultureStore;

        public StringLocalizerFactory(IAdminStore<LocalizedResource> store, IAdminStore<Culture> cultureStore)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cultureStore = cultureStore ?? throw new ArgumentNullException(nameof(cultureStore));
        }

        public IEnumerable<string> CulturesNames 
            => _cultureStore.GetAsync(new PageRequest
            {
                Select = nameof(Culture.Id)
            }).ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
            .Items
            .Select(c => c.Id);

        public IStringLocalizer Create(Type resourceSource)
        {
            var type = typeof(StringLocalizer<>).MakeGenericType(new Type[] { resourceSource });
            return Activator.CreateInstance(type, _store) as IStringLocalizer;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer(_store);
        }
    }
}
