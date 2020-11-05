// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IStringLocalizerFactory" />
    /// <seealso cref="ISupportCultures" />
    public class StringLocalizerFactory : IStringLocalizerFactory, ISupportCultures
    {
        private readonly IServiceProvider _provider;
        private IEnumerable<string> _cultureNames;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizerFactory"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">
        /// store
        /// or
        /// cultureStore
        /// </exception>
        public StringLocalizerFactory(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets the cultures names.
        /// </summary>
        /// <value>
        /// The cultures names.
        /// </value>
        public IEnumerable<string> CulturesNames => GetCultureNames();

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
                return Activator.CreateInstance(type, _provider) as IStringLocalizer;
            }

            return new StringLocalizer(_provider, null, null);
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
            return new StringLocalizer(_provider, baseName, location);
        }

        private IEnumerable<string> GetCultureNames()
        {
            if (_cultureNames != null)
            {
                return _cultureNames;
            }

            lock (_provider)
            {
                if (_cultureNames != null)
                {
                    return _cultureNames;
                }

                return GetCultureNamesAsync().GetAwaiter().GetResult();
            }
        }

        private async Task<IEnumerable<string>> GetCultureNamesAsync()
        {
            using var scope = _provider.CreateScope();
            var cultureStore = scope.ServiceProvider.GetRequiredService<IAdminStore<Culture>>();

            var page = await cultureStore.GetAsync(new PageRequest
            {
                Select = nameof(Culture.Id)
            }).ConfigureAwait(false);

            var culturesList = new List<string> { "en" };
            culturesList.AddRange(page
                .Items
                .Select(c => c.Id));
            _cultureNames =  culturesList.Distinct();

            return _cultureNames;
        }
    }
}
