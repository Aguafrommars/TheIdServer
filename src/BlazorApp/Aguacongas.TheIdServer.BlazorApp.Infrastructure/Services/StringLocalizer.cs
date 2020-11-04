// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services
{
    public class StringLocalizer : ISharedStringLocalizerAsync
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly IAdminStore<Culture> _cultureStore;
        private readonly ILogger<StringLocalizer> _logger;
        private Dictionary<string, LocalizedString> _keyValuePairs = new Dictionary<string, LocalizedString>();
        private IEnumerable<LocalizedResource> _resources;
        private CultureInfo _currentCulture = new CultureInfo("en");

        public event Action ResourceReady;

        public StringLocalizer(IAdminStore<LocalizedResource> store,
            IAdminStore<Culture> cultureStore,
            ILogger<StringLocalizer> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cultureStore = cultureStore ?? throw new ArgumentNullException(nameof(cultureStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public LocalizedString this[string name]
        {
            get
            {
                return GetLocalizedString(name);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                return GetLocalizedString(name, arguments);
            }
        }

        public Task Reset()
        {
            _keyValuePairs = new Dictionary<string, LocalizedString>();
            _resources = null;
            return GetAllResourcesAsync();
        }

        public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
        {
            var response = await _cultureStore.GetAsync(new PageRequest
            {
                Select = nameof(Culture.Id),
                OrderBy = nameof(Culture.Id)
            }).ConfigureAwait(false);

            var cultureList = new List<string>
            {
                "en"
            };
            cultureList.AddRange(response.Items.Select(c => c.Id));
            return cultureList.Distinct();
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            if (_resources != null)
            {
                return _keyValuePairs.Values;
            }

            GetAllResourcesAsync().GetAwaiter().GetResult();
            return _keyValuePairs.Values;
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            return new StringLocalizer(_store, _cultureStore, _logger);
        }

        private LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            if (!_keyValuePairs.TryAdd(name, null))
            {
                var localizedString = new LocalizedString(name, string.Format(_keyValuePairs[name] ?? name, arguments), _keyValuePairs[name] == null);
                if (localizedString.ResourceNotFound && _currentCulture.Name != "en")
                {
                    _logger.LogWarning($"Localized value for key '{name}' not found for culture '{_currentCulture.Name}'");
                }
                return localizedString;
            }
            GetStringAsync(name).ContinueWith(t => SetResource(name, t));
            return new LocalizedString(name, string.Format(name, arguments), true);
        }

        private async Task<LocalizedString> GetStringAsync(string key)
        {
            if (_resources != null)
            {
                var loaded = _resources.FirstOrDefault(r => r.Key == key)?.Value;
                return new LocalizedString(key, loaded ?? key, loaded == null);
            }
            await GetAllResourcesAsync().ConfigureAwait(false);
            var value = _resources.FirstOrDefault(r => r.Key == key)?.Value;
            return new LocalizedString(key, value ?? key, value == null);
        }

        private async Task GetAllResourcesAsync()
        {
            _resources = new LocalizedResource[0];

            _currentCulture = CultureInfo.CurrentCulture;
            var parent = _currentCulture.Parent;
            var filter = $"{nameof(LocalizedResource.CultureId)} eq '{_currentCulture.Name}'";
            if (parent != null)
            {
                filter += $" or {nameof(LocalizedResource.CultureId)} eq '{_currentCulture.Parent.Name}'";
            }

            var page = await _store.GetAsync(new PageRequest
            {
                Filter = filter,
                OrderBy = nameof(LocalizedResource.CultureId)
            }).ConfigureAwait(false);

            _resources = page.Items;
            foreach (var resource in _resources)
            {
                _keyValuePairs[resource.Key] = new LocalizedString(resource.Key, resource.Value ?? resource.Key, resource.Value == null);
            }

            ResourceReady();
        }

        private void SetResource(string name, Task<LocalizedString> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            _keyValuePairs[name] = task.Result;
        }
    }

    public class StringLocalizer<T> : IStringLocalizerAsync<T>, IDisposable
    {
        private readonly ISharedStringLocalizerAsync _sharedStringLocalizer;
        private bool disposedValue;

        public StringLocalizer(ISharedStringLocalizerAsync sharedStringLocalizer) 
        {
            _sharedStringLocalizer = sharedStringLocalizer ?? throw new ArgumentNullException(nameof(sharedStringLocalizer));
            _sharedStringLocalizer.ResourceReady += SharedStringLocalizer_ResourceReady;
        }

        public LocalizedString this[string name] => _sharedStringLocalizer[name];

        public LocalizedString this[string name, params object[] arguments] => _sharedStringLocalizer[name, arguments];

        public Action OnResourceReady { get; set; }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
            => _sharedStringLocalizer.GetAllStrings(includeParentCultures);

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            return new StringLocalizer<T>(_sharedStringLocalizer);
        }


        private void SharedStringLocalizer_ResourceReady()
        {
            OnResourceReady?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sharedStringLocalizer.ResourceReady -= SharedStringLocalizer_ResourceReady;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    [SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "Create an instance by T")]
    public class SharedStringLocalizer<T> : StringLocalizer
    {
        public SharedStringLocalizer(IAdminStore<LocalizedResource> store,
            IAdminStore<Culture> cultureStore, 
            ILogger<SharedStringLocalizer<T>> logger)
            : base(store, cultureStore, logger)
        {
        }
    }
}
