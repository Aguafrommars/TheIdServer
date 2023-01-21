// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class TokenCleanerHostTest
    {
        [Fact]
        public async Task StartAsync_should_start_token_cleaner_task()
        {
            using var resetEvent = new ManualResetEvent(false);
            var oneTimeTokenStoreMock = new Mock<IAdminStore<OneTimeToken>>();

            oneTimeTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PageResponse<OneTimeToken>
                {
                    Items = Array.Empty<OneTimeToken>()
                });

            var refreshTokenStoreMock = new Mock<IAdminStore<RefreshToken>>();

            refreshTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PageResponse<RefreshToken>
                {
                    Items = Array.Empty<RefreshToken>()
                });


            var referenceTokenStoreMock = new Mock<IAdminStore<ReferenceToken>>();

            referenceTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PageResponse<ReferenceToken>
                {
                    Items = new[]
                    {
                        new ReferenceToken()
                    }
                });

            referenceTokenStoreMock.Setup(m => m.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Callback(() => resetEvent.Set())
                .Returns(Task.CompletedTask);

            var provider = new ServiceCollection().AddLogging()
                .AddTransient(p => oneTimeTokenStoreMock.Object)
                .AddTransient(p => referenceTokenStoreMock.Object)
                .AddTransient(p => refreshTokenStoreMock.Object)
                .BuildServiceProvider();

            using var sut = new TokenCleanerHost(provider, TimeSpan.FromSeconds(1), provider.GetRequiredService<ILogger<TokenCleanerHost>>());

            await sut.StartAsync(default);

            if (Debugger.IsAttached)
            {
                resetEvent.WaitOne();
            }

            Assert.True(resetEvent.WaitOne(TimeSpan.FromSeconds(10)));

            await sut.StopAsync(default);
        }

        [Fact]
        public async Task StartAsync_should_exit_on_cancel()
        {
            using var resetEvent = new ManualResetEvent(false);
            using var tokenSource = new CancellationTokenSource();
            var oneTimeTokenStoreMock = new Mock<IAdminStore<OneTimeToken>>();

            oneTimeTokenStoreMock.Setup(m => m.GetAsync(It.IsAny<PageRequest>(), It.IsAny<CancellationToken>()))
                .Callback(() =>
                {
                    tokenSource.Cancel();
                    resetEvent.Set();
                }).Throws(new TaskCanceledException());

            var refreshTokenStoreMock = new Mock<IAdminStore<RefreshToken>>();
            var referenceTokenStoreMock = new Mock<IAdminStore<ReferenceToken>>();
            var provider = new ServiceCollection().AddLogging()
                .AddTransient(p => oneTimeTokenStoreMock.Object)
                .AddTransient(p => referenceTokenStoreMock.Object)
                .AddTransient(p => refreshTokenStoreMock.Object)
                .BuildServiceProvider();

            using var sut = new TokenCleanerHost(provider, TimeSpan.FromSeconds(1), provider.GetRequiredService<ILogger<TokenCleanerHost>>());

            await sut.StartAsync(tokenSource.Token);

            if (Debugger.IsAttached)
            {
                resetEvent.WaitOne();
            }

            Assert.True(resetEvent.WaitOne(TimeSpan.FromSeconds(10)));
        }
    }
}
