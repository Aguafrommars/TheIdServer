// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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
    public class SchemeChangeHost : BackgroundService
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeChangeHost"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <exception cref="ArgumentNullException">subscriber</exception>
        public SchemeChangeHost(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }


        /// <summary>
        /// This method is called when the <see cref="T:Microsoft.Extensions.Hosting.IHostedService" /> starts. The implementation should return a task that represents
        /// the lifetime of the long running operation(s) being performed.
        /// </summary>
        /// <param name="stoppingToken">Triggered when <see cref="M:Microsoft.Extensions.Hosting.IHostedService.StopAsync(System.Threading.CancellationToken)" /> is called.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _provider.CreateScope();
            var subscriber = scope.ServiceProvider.GetRequiredService<ISchemeChangeSubscriber>();
            await Task.Run(() => subscriber.SubscribeAsync(stoppingToken), stoppingToken).ConfigureAwait(false); // release the calling thread.
            stoppingToken.WaitHandle.WaitOne();
            await subscriber.UnSubscribeAsync(default).ConfigureAwait(false);
        }
    }
}
