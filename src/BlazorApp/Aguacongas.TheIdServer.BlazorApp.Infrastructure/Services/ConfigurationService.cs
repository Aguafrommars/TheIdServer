using Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicConfiguration = Aguacongas.DynamicConfiguration.Razor.Services;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class ConfigurationService : DynamicConfiguration.Razor.Services.IConfigurationService
    {
        private readonly DynamicConfiguration.Razor.Services.ConfigurationService _parent;
        private readonly Notifier _notifier;
        private readonly IStringLocalizerAsync<ConfigurationService> _localizer;

        public object Configuration => _parent.Configuration;

        public ConfigurationService(DynamicConfiguration.Razor.Services.ConfigurationService parent, Notifier notifier, IStringLocalizerAsync<ConfigurationService> localizer)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public Task<object> GetAsync(string key, CancellationToken cancellationToken = default)
        => _parent.GetAsync(key, cancellationToken);

        public async Task SaveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _parent.SaveAsync(key,cancellationToken).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                await _notifier.NotifyAsync(new Models.Notification
                {
                    Header = _localizer["Error"],
                    IsError = true,
                    Message = ex.Message
                }).ConfigureAwait(false);
                return;
            }

            await _notifier.NotifyAsync(new Models.Notification
            {
                Header = _localizer["Settings"],
                Message = _localizer["Saved"]
            }).ConfigureAwait(false);
        }
    }
}
