// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IStringLocalizer" />
    public class StringLocalizer : IStringLocalizer, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly IServiceScope _scope;
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly string _baseName;
        private readonly string _location;
        private readonly ILogger<StringLocalizer> _logger;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizer"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="baseName">Name of the base.</param>
        /// <param name="location">The location.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public StringLocalizer(IServiceProvider provider, string baseName, string location)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _scope = provider.CreateScope();
            var p = _scope.ServiceProvider;
            _store = p.GetRequiredService<IAdminStore<LocalizedResource>>();
            _baseName = baseName;
            _location = location;
            _logger = p.GetRequiredService<ILogger<StringLocalizer>>();
        }

        /// <summary>
        /// Gets the <see cref="LocalizedString"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="LocalizedString"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        /// <summary>
        /// Gets the <see cref="LocalizedString"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="LocalizedString"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns></returns>
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" /> for a specific <see cref="T:System.Globalization.CultureInfo" />.
        /// </summary>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use.</param>
        /// <returns>
        /// A culture-specific <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" />.
        /// </returns>
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            CultureInfo.DefaultThreadCurrentCulture = culture;
            return new StringLocalizer(_provider, _baseName, _location);
        }

        /// <summary>
        /// Gets all string resources.
        /// </summary>
        /// <param name="includeParentCultures">A <see cref="T:System.Boolean" /> indicating whether to include strings from parent cultures.</param>
        /// <returns>
        /// The strings.
        /// </returns>
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return GetAllStringsAsync(includeParentCultures).GetAwaiter().GetResult();
        }

        private string GetString(string name)
        {
            return GetStringAsync(name).GetAwaiter().GetResult();
        }

        private async Task<string> GetStringAsync(string name)
        {
            var currentCulture = CultureInfo.CurrentCulture;
            var filter = $"{nameof(LocalizedResource.CultureId)} eq '{currentCulture.Name}' and {nameof(LocalizedResource.Key)} eq '{name.Replace("'", "''")}'";
            if (!string.IsNullOrEmpty(_baseName))
            {
                filter += $" and ({nameof(LocalizedResource.BaseName)} eq null or {nameof(LocalizedResource.BaseName)} eq '{_baseName}')";
            }
            if (!string.IsNullOrEmpty(_location))
            {
                filter += $" and ({nameof(LocalizedResource.Location)} eq null or {nameof(LocalizedResource.Location)} eq '{_location}')";
            }

            var response = await _store.GetAsync(new PageRequest
            {
                Take = 1,
                Filter = filter,
                OrderBy = $"{nameof(LocalizedResource.BaseName)} desc,{nameof(LocalizedResource.Location)} desc"
            }).ConfigureAwait(false);

            if (response.Count == 0 && currentCulture.Name != "en")
            {
                _logger.LogWarning("Key {Key} not found for Culture {Culture}", name, currentCulture);
            }
            return response.Items.Select(i => new LocalizedString(i.Key, i.Value)).FirstOrDefault();
        }

        private async Task<IEnumerable<LocalizedString>> GetAllStringsAsync(bool includeParentCultures)
        {
            var select = $"{nameof(LocalizedResource.CultureId)} eq '{CultureInfo.CurrentCulture.Name}'";
            if (includeParentCultures && CultureInfo.CurrentCulture.Parent != null)
            {
                select += $" or {nameof(LocalizedResource.CultureId)} eq '{CultureInfo.CurrentCulture.Parent.Name}'";
            }
            var response = await _store.GetAsync(new PageRequest
            {
                Take = null,
                Select = select
            }).ConfigureAwait(false);

            return response.Items.Select(i => new LocalizedString(i.Key, i.Value));
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _scope.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="IStringLocalizer" />
    public class StringLocalizer<T> : StringLocalizer, IStringLocalizer<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizer{T}"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public StringLocalizer(IServiceProvider provider) : base(provider, typeof(T).FullName, typeof(T).Namespace)
        {
        }
    }
}
