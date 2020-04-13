using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    class HubConnectionFactory: IDisposable
    {
        private readonly IConfiguration _configuration;
        private HubConnection _hubConnection;
        private bool disposedValue;

        public HubConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public HubConnection GetConnection()
        {
            var hubUrl = _configuration.GetValue<string>("SignalR:HubUrl");

            if (string.IsNullOrEmpty(hubUrl))
            {
                return null;
            }

            var builder = new HubConnectionBuilder()
                .WithUrl(hubUrl)
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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            StartConnectionAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
