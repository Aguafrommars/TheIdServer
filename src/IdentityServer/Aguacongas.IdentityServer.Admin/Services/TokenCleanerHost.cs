using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Clean tokens.
    /// </summary>
    /// <seealso cref="IHostedService" />
    public class TokenCleanerHost : IHostedService
    {
        private readonly IServiceProvider _provider;
        private readonly TimeSpan _interval;
        private readonly ILogger<TokenCleanerHost> _logger;
        private CancellationTokenSource _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenCleanerHost" /> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">provider</exception>
        public TokenCleanerHost(IServiceProvider provider, TimeSpan interval, ILogger<TokenCleanerHost> logger)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _interval = interval;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_source != null)
            {
                throw new InvalidOperationException("Already started. Call Stop first.");
            }

            _logger.LogInformation("Starting tokens removal");

            _source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            return Task.Factory.StartNew(() => CleanupAsync(_source.Token));
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_source == null)
            {
                throw new InvalidOperationException("Not started. Call Start first.");
            }

            _logger.LogInformation("Stopping tokens removal");

            _source.Cancel();
            _source = null;

            return Task.CompletedTask;
        }

        private async Task CleanupAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                try
                {
                    await Task.Delay(_interval, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    _logger.LogDebug("TaskCanceledException. Exiting.");
                    break;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Task.Delay exception: {e.Message}. Exiting.");
                    break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("CancellationRequested. Exiting.");
                    break;
                }

                await RemoveExpiredTokensAsync();
            }
        }

        private async Task RemoveExpiredTokensAsync()
        {
            try
            {
                using var scope = _provider.CreateScope();
                var service = scope.ServiceProvider;
                var oneTimeTokenStore = service.GetRequiredService<IAdminStore<OneTimeToken>>();
                var oneTimeTokenResponse = await oneTimeTokenStore.GetAsync(new PageRequest
                {
                    Filter = $"{nameof(IAuditable.CreatedAt)} gt {DateTime.UtcNow.AddMinutes(1)}",
                    OrderBy = nameof(IAuditable.CreatedAt)
                }).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(oneTimeTokenStore, oneTimeTokenResponse.Items).ConfigureAwait(false);

                var pageRequest = new PageRequest
                {
                    Filter = $"{nameof(IGrant.Expiration)} gt {DateTime.UtcNow}",
                    OrderBy = nameof(IAuditable.CreatedAt)
                };
                var refreshTokenStore = service.GetRequiredService<IAdminStore<ReferenceToken>>();
                var refreshTokenResponse = await refreshTokenStore.GetAsync(pageRequest).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(refreshTokenStore, refreshTokenResponse.Items).ConfigureAwait(false);

                var referenceTokenStore = service.GetRequiredService<IAdminStore<ReferenceToken>>();
                var referenceTokenResponse = await referenceTokenStore.GetAsync(pageRequest).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(referenceTokenStore, referenceTokenResponse.Items).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception removing expired grants: {e.Message}");

            }
        }

        private async Task RemoveExpiredTokensAsync(IAdminStore store, IEnumerable<IGrant> items)
        {
            foreach (var token in items)
            {
                await store.DeleteAsync(token.Id).ConfigureAwait(false);
            }
        }
    }
}
