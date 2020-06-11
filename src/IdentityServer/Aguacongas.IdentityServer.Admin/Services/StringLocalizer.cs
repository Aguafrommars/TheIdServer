using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.Localization;
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
    public class StringLocalizer : IStringLocalizer
    {
        private readonly IAdminStore<LocalizedResource> _store;
        private readonly string _baseName;
        private readonly string _location;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringLocalizer"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="baseName">Name of the base.</param>
        /// <param name="location">The location.</param>
        /// <exception cref="ArgumentNullException">store</exception>
        public StringLocalizer(IAdminStore<LocalizedResource> store, string baseName, string location)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _baseName = baseName;
            _location = location;
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
            return new StringLocalizer(_store, _baseName, _location);
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
            var select = $"{nameof(LocalizedResource.CultureId)} eq '{CultureInfo.CurrentCulture.Name}' and {nameof(LocalizedResource.Key)} eq '{name.Replace("'", "''")}'";
            if (!string.IsNullOrEmpty(_baseName))
            {
                select += $" and ({nameof(LocalizedResource.BaseName)} eq null or {nameof(LocalizedResource.BaseName)} eq '{_baseName}')";
            }
            if (!string.IsNullOrEmpty(_location))
            {
                select += $" and ({nameof(LocalizedResource.Location)} eq null or {nameof(LocalizedResource.Location)} eq '{_location}')";
            }

            var response = await _store.GetAsync(new PageRequest
            {
                Take = 1,
                Select = select
            }).ConfigureAwait(false);

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
        /// <param name="store">The store.</param>
        public StringLocalizer(IAdminStore<LocalizedResource> store) : base(store, typeof(T).FullName, typeof(T).Namespace)
        {
        }
    }
}
