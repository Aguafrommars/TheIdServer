// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aguacongas.IdentityServer.Http.Store
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly IAdminStore<LocalizedResource> _store;

        public StringLocalizer(IAdminStore<LocalizedResource> store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new StringLocalizer(_store);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var page = _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(LocalizedResource.CultureId)} eq '{CultureInfo.CurrentCulture.Name}'",
                Select = $"{nameof(LocalizedResource.Key)},{nameof(LocalizedResource.Value)}"
            }).ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

            return page.Items
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            var page = _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(LocalizedResource.Culture)} eq '{CultureInfo.CurrentCulture.Name}' and {nameof(LocalizedResource.Key)} eq '{name}'",
                Select = nameof(LocalizedResource.Value)
            }).ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

            return page.Items
                .FirstOrDefault()?.Value;
        }

    }

    public class StringLocalizer<T> : StringLocalizer, IStringLocalizer<T>
    {
        public StringLocalizer(IAdminStore<LocalizedResource> store) : base(store)
        {
        }
    }
}
