using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class StringLocalizer : IStringLocalizer
    {
        private readonly ConfigurationDbContext _context;
        private readonly string _baseName;
        private readonly string _location;

        public StringLocalizer(ConfigurationDbContext context, string baseName, string location)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _baseName = baseName;
            _location = location;
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
            return new StringLocalizer(_context, _baseName, _location);
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _context.LocalizedResources
                .Include(r => r.Culture)
                .Where(r => r.Culture.Id == CultureInfo.CurrentCulture.Name)
                .Select(r => new LocalizedString(r.Key, r.Value, true));
        }

        private string GetString(string name)
        {
            return _context.LocalizedResources
                .Include(r => r.Culture)
                .FirstOrDefault(r => r.Culture.Id == CultureInfo.CurrentCulture.Name &&
                    r.Key == name &&
                    (r.BaseName == null || r.BaseName == _baseName) &&
                    (r.Location == null || r.Location == _location))?.Value;
        }

    }

    public class StringLocalizer<T> : StringLocalizer, IStringLocalizer<T>
    {
        public StringLocalizer(ConfigurationDbContext context) : base(context, typeof(T).FullName, typeof(T).Namespace)
        {
        }
    }
}
