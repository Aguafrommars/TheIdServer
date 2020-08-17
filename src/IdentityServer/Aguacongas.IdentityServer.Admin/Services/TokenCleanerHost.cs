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
    public class TokenCleanerHost : IHostedService, IDisposable
    {
        private readonly IServiceProvider _provider;
        private readonly TimeSpan _interval;
        private readonly ILogger<TokenCleanerHost> _logger;
        private CancellationTokenSource _source;
        private bool disposedValue;

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
            _logger.LogInformation("Starting tokens removal");

            Cancel();
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
            _logger.LogInformation("Stopping tokens removal");

            Cancel();

            return Task.CompletedTask;
        }

        private void Cancel()
        {
            _source?.Cancel();
            _source = null;
        }

        private async Task CleanupAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
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

                await RemoveExpiredTokensAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task RemoveExpiredTokensAsync(CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var service = scope.ServiceProvider;
                var pageRequest = new PageRequest
                {
                    // Community OData transform date from UTC so we substract the UTC diff
                    Filter = $"{nameof(IGrant.Expiration)} lt {DateTime.UtcNow.AddHours(DateTime.UtcNow.Hour - DateTime.Now.Hour):o}",
                    OrderBy = nameof(IAuditable.CreatedAt)
                };

                var oneTimeTokenStore = service.GetRequiredService<IAdminStore<OneTimeToken>>();
                var oneTimeTokenResponse = await oneTimeTokenStore.GetAsync(pageRequest, cancellationToken).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(oneTimeTokenStore, oneTimeTokenResponse.Items, cancellationToken).ConfigureAwait(false);

                var refreshTokenStore = service.GetRequiredService<IAdminStore<RefreshToken>>();
                var refreshTokenResponse = await refreshTokenStore.GetAsync(pageRequest, cancellationToken).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(refreshTokenStore, refreshTokenResponse.Items, cancellationToken).ConfigureAwait(false);

                var referenceTokenStore = service.GetRequiredService<IAdminStore<ReferenceToken>>();
                var referenceTokenResponse = await referenceTokenStore.GetAsync(pageRequest, cancellationToken).ConfigureAwait(false);
                await RemoveExpiredTokensAsync(referenceTokenStore, referenceTokenResponse.Items, cancellationToken).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("TaskCanceledException. Exiting.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception removing expired grants: {e.Message}");

            }
        }

        private async Task RemoveExpiredTokensAsync(IAdminStore store, IEnumerable<IGrant> items, CancellationToken cancellationToken)
        {
            foreach (var token in items)
            {
                await store.DeleteAsync(token.Id, cancellationToken).ConfigureAwait(false);
            }
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
                    _source?.Dispose();
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
}
