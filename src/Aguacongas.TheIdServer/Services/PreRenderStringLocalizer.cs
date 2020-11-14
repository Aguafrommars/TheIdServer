using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.Services
{
    public class PreRenderStringLocalizer : StringLocalizer
    {
        public PreRenderStringLocalizer(IAdminStore<LocalizedResource> store, IAdminStore<Culture> cultureStore, ILogger<StringLocalizer> logger) : base(store, cultureStore, logger)
        {
            GetSupportedCulturesAsync().GetAwaiter().GetResult();
        }

        protected override LocalizedString GetLocalizedString(string name, params object[] arguments)
        {
            if (!KeyValuePairs.TryAdd(name, GetStringAsync(name).GetAwaiter().GetResult()))
            {
                var value = KeyValuePairs[name];
                var localizedString = new LocalizedString(name, string.Format(value ?? name, arguments), value == null);
                if (localizedString.ResourceNotFound && CurrentCulture.Name != "en")
                {
                    Logger.LogWarning($"Localized value for key '{name}' not found for culture '{CurrentCulture.Name}'");
                }
                return localizedString;
            }
            return new LocalizedString(name, string.Format(KeyValuePairs[name], arguments), true);
        }
    }
}
