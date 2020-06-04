using Aguacongas.IdentityServer.Abstractions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class StringLocalizerFactory : IStringLocalizerFactory, ISupportCultures, IDisposable
    {
        private readonly ConfigurationDbContext _context;
        private bool disposedValue;

        public StringLocalizerFactory(ConfigurationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<string> CulturesNames 
            => _context.Cultures.Select(c => c.Id);

        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource != null)
            {
                var type = typeof(StringLocalizer<>).MakeGenericType(new Type[] { resourceSource });
                return Activator.CreateInstance(type, _context) as IStringLocalizer;
            }

            return new StringLocalizer(_context, null, null);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new StringLocalizer(_context, baseName, location);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context?.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
