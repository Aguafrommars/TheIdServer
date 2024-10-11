// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Services;
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
        private readonly IReadOnlyLocalizedResourceStore _store;
        private readonly IReadOnlyCultureStore _cultureStore;
        private IEnumerable<LocalizedResource> _resources;
        private IEnumerable<string> _cultureList;

        protected Dictionary<string, LocalizedString> KeyValuePairs { get; set; } = new Dictionary<string, LocalizedString>(StringComparer.OrdinalIgnoreCase);

        protected ILogger<StringLocalizer> Logger { get; }
        public CultureInfo CurrentCulture { get; set; } = new CultureInfo("en");

        public event Action ResourceReady;

        public StringLocalizer(IReadOnlyLocalizedResourceStore store,
            IReadOnlyCultureStore cultureStore,
            ILogger<StringLocalizer> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cultureStore = cultureStore ?? throw new ArgumentNullException(nameof(cultureStore));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            KeyValuePairs = new Dictionary<string, LocalizedString>();
            _resources = null;
            return GetAllResourcesAsync();
        }

        public async Task<IEnumerable<string>> GetSupportedCulturesAsync()
        {
            if (_cultureList != null)
            {
                return _cultureList;
            }

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
            _cultureList = cultureList.Distinct();
            return _cultureList;
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            if (_resources != null)
            {
                return KeyValuePairs.Values;
            }

            GetAllResourcesAsync().GetAwaiter().GetResult();
            return KeyValuePairs.Values;
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            return new StringLocalizer(_store, _cultureStore, Logger);
        }

        protected virtual LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            if (!KeyValuePairs.TryAdd(name, null) && KeyValuePairs.TryGetValue(name, out LocalizedString value))
            {
                var localizedString = new LocalizedString(name, string.Format(value ?? name, arguments), value == null);
                if (localizedString.ResourceNotFound && CurrentCulture.Name != "en")
                {
                    Logger.LogWarning("Localized value for key '{Name}' not found for culture '{CurrentCultureName}'", name, CurrentCulture.Name);
                }
                return localizedString;
            }
            GetStringAsync(name).ContinueWith(t => SetResource(name, t));
            return new LocalizedString(name, string.Format(name, arguments), true);
        }

        protected async Task<LocalizedString> GetStringAsync(string key)
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

        protected virtual async Task GetAllResourcesAsync()
        {
            _resources = Array.Empty<LocalizedResource>();

            CurrentCulture = CultureInfo.CurrentCulture;
            var parent = CurrentCulture.Parent;
            var filter = $"{nameof(LocalizedResource.CultureId)} eq '{CurrentCulture.Name}'";
            if (!string.IsNullOrWhiteSpace(parent?.Name))
            {
                filter += $" or {nameof(LocalizedResource.CultureId)} eq '{parent.Name}'";
            }

            var page = await _store.GetAsync(new PageRequest
            {
                Filter = filter,
                OrderBy = nameof(LocalizedResource.CultureId)
            }).ConfigureAwait(false);

            _resources = page.Items;
            foreach (var resource in _resources)
            {
                KeyValuePairs[resource.Key] = new LocalizedString(resource.Key, resource.Value ?? resource.Key, resource.Value == null);
            }

            ResourceReady?.Invoke();
        }

        private void SetResource(string name, Task<LocalizedString> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            KeyValuePairs[name] = task.Result;
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
        public SharedStringLocalizer(IReadOnlyLocalizedResourceStore store,
            IReadOnlyCultureStore cultureStore, 
            ILogger<SharedStringLocalizer<T>> logger)
            : base(store, cultureStore, logger)
        {
        }
    }
}
