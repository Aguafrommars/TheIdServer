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
    public class StringLocalizer : IStringLocalizerAsync, ISharedStringLocalizerAsync
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly Dictionary<string, string> _keyValuePairs = new Dictionary<string, string>();
        public event Action ResourceReady;

        public StringLocalizer(HttpClient client, ILogger<AdminStore<LocalizedResource>> logger)
        {
            _store = new AdminStore<LocalizedResource>(Task.FromResult(client), logger);
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

        private async Task<string> GetStringAsync(string key)
        {
            var cultureName = CultureInfo.CurrentCulture.Name;
            var page = await _store.GetAsync(new PageRequest
            {
                Filter = $"{nameof(LocalizedResource.CultureId)} eq '{cultureName}' and {nameof(LocalizedResource.Key)} eq '{key}'",
                Select = nameof(LocalizedResource.Value),
                Take = 1
            }).ConfigureAwait(false);

            return page.Items
                .FirstOrDefault()?.Value;
        }

        private void SetResource(string name, Task<string> task)
        {
            if (task.Exception != null)
            {
                return;
            }

            _keyValuePairs[name] = task.Result;
            ResourceReady();
        }
    }

    public class StringLocalizer<T> : IStringLocalizerAsync<T>, IDisposable
    {
        private readonly SharedStringLocalizer<T> _sharedStringLocalizer;
        private bool disposedValue;

        public StringLocalizer(SharedStringLocalizer<T> sharedStringLocalizer) 
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
        public SharedStringLocalizer(IHttpClientFactory factory, ILogger<AdminStore<LocalizedResource>> logger)
            : base(factory.CreateClient("localizer"), logger)
        {
        }
    }
}
