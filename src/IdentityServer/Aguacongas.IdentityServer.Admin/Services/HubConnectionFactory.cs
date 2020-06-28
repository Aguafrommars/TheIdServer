using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Hub connection factory
    /// </summary>
    /// <seealso cref="IDisposable" />
    public class HubConnectionFactory : IDisposable
    {
        private readonly object _syncLock = new object();
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _provider;
        private readonly ILogger<HubConnectionFactory> _logger;
        private HubConnection _hubConnection;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubConnectionFactory"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="provider">The service provider</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">configuration</exception>
        public HubConnectionFactory(IConfiguration configuration, IServiceProvider provider, ILogger<HubConnectionFactory> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public HubConnection GetConnection(CancellationToken cancellationToken)
        {
            var hubUrl = _configuration.GetValue<string>("SignalR:HubUrl");

            if (string.IsNullOrEmpty(hubUrl))
            {
                return null;
            }

            if (_hubConnection != null)
            {
                return _hubConnection;
            }

            lock (_syncLock)
            {
                if (_hubConnection != null)
                {
                    return _hubConnection;
                }

                var builder = new HubConnectionBuilder()
                    .WithUrl(hubUrl, options =>
                    {
                        options.HttpMessageHandlerFactory = _ => _provider.GetRequiredService<HubHttpMessageHandlerAccessor>().Handler;
                        options.AccessTokenProvider = async () =>
                        {
                            var manager = _provider.GetRequiredService<OAuthTokenManager>();
                            var token = await manager.GetTokenAsync().ConfigureAwait(false);
                            return token.Parameter;
                        };
                    })
                    .WithAutomaticReconnect();

                if (_configuration.GetValue<bool>("SignalR:UseMessagePack"))
                {
                    builder.AddMessagePackProtocol();
                }

                _hubConnection = builder.Build();

                _hubConnection.Closed += (error) =>
                {
                    return StartConnectionAsync(cancellationToken);
                };

                return _hubConnection;
            }
        }

        /// <summary>
        /// Starts the connection asynchronous.
        /// </summary>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "infinite auto reconnection")]
        public async Task StartConnectionAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (_hubConnection == null)
                {
                    return;
                }

                try
                {
                    await _hubConnection.StartAsync(cancellationToken).ConfigureAwait(false);
                    Debug.Assert(_hubConnection.State == HubConnectionState.Connected);
                    return;
                }
                catch(ObjectDisposedException)
                {
                    return;
                }
                catch(Exception e)
                {
                    // Failed to connect, trying again in 5000 ms.
                    _logger.LogError(e, e.Message);
                    await Task.Delay(5000).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Stops the connection asynchronous.
        /// </summary>
        /// <returns></returns>
        public Task StopConnectionAsync(CancellationToken cancellationToken)
        {
            if (_hubConnection != null)
            {
                return _hubConnection.StopAsync(cancellationToken);
            }
            return Task.CompletedTask;
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
                    _hubConnection?.DisposeAsync().GetAwaiter().GetResult();
                    _hubConnection = null;
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
