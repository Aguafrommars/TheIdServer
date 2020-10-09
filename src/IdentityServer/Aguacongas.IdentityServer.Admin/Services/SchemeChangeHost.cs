// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IHostedService" />
    public class SchemeChangeHost : IHostedService, IDisposable
    {
        private readonly IServiceScope _scope;
        private readonly ISchemeChangeSubscriber _subscriber;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeChangeHost"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">subscriber</exception>
        public SchemeChangeHost(IServiceProvider provider)
        {
            provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _scope = provider.CreateScope();
            _subscriber = _scope.ServiceProvider.GetRequiredService<ISchemeChangeSubscriber>();
        }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Delay(500).ContinueWith(_ =>  _subscriber.SubscribeAsync(cancellationToken));
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful shutdown.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _subscriber.UnSubscribeAsync(cancellationToken);
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
                    _scope.Dispose();
                }
                disposedValue = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
