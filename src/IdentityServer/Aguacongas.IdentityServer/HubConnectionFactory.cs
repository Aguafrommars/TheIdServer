using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer
{
    /// <summary>
    /// Hub connection factory
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class HubConnectionFactory : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _provider;
        private HubConnection _hubConnection;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="HubConnectionFactory"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="provider">The service provider</param>
        /// <exception cref="ArgumentNullException">configuration</exception>
        public HubConnectionFactory(IConfiguration configuration, IServiceProvider provider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <returns></returns>
        public HubConnection GetConnection()
        {
            var hubUrl = _configuration.GetValue<string>("SignalR:HubUrl");

            if (string.IsNullOrEmpty(hubUrl))
            {
                return null;
            }

            var builder = new HubConnectionBuilder()
                .WithUrl(hubUrl, options =>
                {
                    options.HttpMessageHandlerFactory = _ => _provider.GetRequiredService<HttpClientHandler>();
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
                return StartConnectionAsync();
            };

            StartConnectionAsync().GetAwaiter().GetResult();

            return _hubConnection;

        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "infinite auto reconnection")]
        private async Task StartConnectionAsync()
        {
            while (true)
            {
                if (_hubConnection == null)
                {
                    return;
                }

                try
                {
                    await _hubConnection.StartAsync().ConfigureAwait(false);
                    Debug.Assert(_hubConnection.State == HubConnectionState.Connected);
                    return;
                }
                catch
                {
                    // Failed to connect, trying again in 5000 ms.
                    Debug.Assert(_hubConnection.State == HubConnectionState.Disconnected);
                    await Task.Delay(5000).ConfigureAwait(false);
                }
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
