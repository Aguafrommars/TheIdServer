using Aguacongas.IdentityServer.Admin.Http.Store;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services
{
    public class StringLocalizer : ISharedStringLocalizerAsync
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly IAdminStore<Culture> _cultureStore;
        private Dictionary<string, string> _keyValuePairs = new Dictionary<string, string>();
        private IEnumerable<LocalizedResource> _resources;
        public event Action ResourceReady;

        public StringLocalizer(HttpClient client, ILogger<AdminStore<LocalizedResource>> resourceLogger, ILogger<AdminStore<Culture>> cultureLogger)
        {
            _store = new AdminStore<LocalizedResource>(Task.FromResult(client), resourceLogger);
            _cultureStore = new AdminStore<Culture>(Task.FromResult(client), cultureLogger);
        }

        public string this[string name]
        {
            get
            {
                if (!_keyValuePairs.TryAdd(name, null))
                {
                    return _keyValuePairs[name] ?? name;
                }
                GetStringAsync(name).ContinueWith(t => SetResource(name, t));
                return name;
            }
        }

        public string this[string name, params object[] arguments]
        {
            get
            {
                if (!_keyValuePairs.TryAdd(name, null))
                {
                    return string.Format(_keyValuePairs[name] ?? name, arguments);
                }
                GetStringAsync(name).ContinueWith(t => SetResource(name, t));
                return string.Format(name, arguments);
            }
        }

        public Task Reset()
        {
            _keyValuePairs = new Dictionary<string, string>();
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
                "en-US"
            };
            cultureList.AddRange(response.Items.Select(c => c.Id));
            return cultureList.Distinct();
        }

        private async Task<string> GetStringAsync(string key)
        {
            if (_resources != null)
            {
                return _resources.FirstOrDefault(r => r.Key == key)?.Value;
            }
            await GetAllResourcesAsync().ConfigureAwait(false);
            return _resources.FirstOrDefault(r => r.Key == key)?.Value;
        }

        private async Task GetAllResourcesAsync()
        {
            var cultureName = CultureInfo.CurrentCulture.Name;

            _resources = new LocalizedResource[0];

            var page = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(LocalizedResource.CultureId)} eq '{cultureName}'"
            }).ConfigureAwait(false);

            _resources = page.Items;
            foreach (var resource in _resources)
            {
                _keyValuePairs[resource.Key] = resource.Value;
            }

            ResourceReady();
        }

        private void SetResource(string name, Task<string> task)
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
            _sharedStringLocalizer.ResourceReady += _sharedStringLocalizer_ResourceReady;
        }

        public string this[string name] => _sharedStringLocalizer[name];

        public string this[string name, params object[] arguments] => _sharedStringLocalizer[name, arguments];

        public Action OnResourceReady { get; set; }

        private void _sharedStringLocalizer_ResourceReady()
        {
            OnResourceReady?.Invoke();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _sharedStringLocalizer.ResourceReady -= _sharedStringLocalizer_ResourceReady;
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
        public SharedStringLocalizer(IHttpClientFactory factory, ILogger<AdminStore<LocalizedResource>> logger, ILogger<AdminStore<Culture>> cultureLogger)
            : base(factory.CreateClient("localizer"), logger, cultureLogger)
        {
        }
    }
}
