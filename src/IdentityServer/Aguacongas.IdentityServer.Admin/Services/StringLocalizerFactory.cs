using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IStringLocalizerFactory" />
    /// <seealso cref="ISupportCultures" />
    public class StringLocalizerFactory : IStringLocalizerFactory, ISupportCultures
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly IAdminStore<Culture> _cultureStore;
        private readonly ILogger<StringLocalizer> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizerFactory"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="cultureStore">The culture store.</param>
        /// <exception cref="ArgumentNullException">
        /// store
        /// or
        /// cultureStore
        /// </exception>
        public StringLocalizerFactory(IAdminStore<LocalizedResource> store, IAdminStore<Culture> cultureStore, ILogger<StringLocalizer> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _cultureStore = cultureStore ?? throw new ArgumentNullException(nameof(cultureStore));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the cultures names.
        /// </summary>
        /// <value>
        /// The cultures names.
        /// </value>
        public IEnumerable<string> CulturesNames
            => _cultureStore.GetAsync(new PageRequest
            {
                Select = nameof(Culture.Id)
            }).ConfigureAwait(false)
            .GetAwaiter()
            .GetResult()
            .Items
            .Select(c => c.Id);

        /// <summary>
        /// Creates an <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" /> using the <see cref="T:System.Reflection.Assembly" /> and
        /// <see cref="P:System.Type.FullName" /> of the specified <see cref="T:System.Type" />.
        /// </summary>
        /// <param name="resourceSource">The <see cref="T:System.Type" />.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" />.
        /// </returns>
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource != null)
            {
                var type = typeof(StringLocalizer<>).MakeGenericType(new Type[] { resourceSource });
                return Activator.CreateInstance(type, _store, _logger) as IStringLocalizer;
            }

            return new StringLocalizer(_store, null, null, _logger);
        }

        /// <summary>
        /// Creates an <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" />.
        /// </summary>
        /// <param name="baseName">The base name of the resource to load strings from.</param>
        /// <param name="location">The location to load resources from.</param>
        /// <returns>
        /// The <see cref="T:Microsoft.Extensions.Localization.IStringLocalizer" />.
        /// </returns>
        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer(_store, baseName, location, _logger);
        }
    }
}
