using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class LetsEncryptServiceTest
    {
        [Fact]
        public async Task CreateCertificateAsync_should_create_certificate_file()
        {
            var mockWebHost = new Mock<IWebHost>();
            mockWebHost.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOrderContext = new Mock<IOrderContext>();
            var mockAuthorizationContext = new Mock<IAuthorizationContext>();
            var mockChallengeContext = new Mock<IChallengeContext>();

            var mockOptions = new Mock<IOptions<CertesAccount>>();

            var certesAccount = new CertesAccount
            {
                Enable = true,
                ServerUrl = "http://test.com",
                AccountDer = Convert.ToBase64String(Convert.FromBase64String("test")),
                PfxPath = Guid.NewGuid().ToString()
            };

            mockOptions.SetupGet(m => m.Value).Returns(certesAccount);

            mockAcmeContext.Setup(m => m.NewOrder(It.IsAny<IList<string>>(), null, null))
                .ReturnsAsync(mockOrderContext.Object);

            mockOrderContext.Setup(m => m.Authorizations()).ReturnsAsync(new IAuthorizationContext[] { mockAuthorizationContext.Object });
            mockAuthorizationContext.Setup(m => m.Resource()).ReturnsAsync(new Authorization
            {
                Status = AuthorizationStatus.Pending
            });
            mockAuthorizationContext.Setup(m => m.Challenges()).ReturnsAsync(new IChallengeContext[] { mockChallengeContext.Object } );
            mockChallengeContext.SetupGet(m => m.KeyAuthz).Returns("test");
            mockChallengeContext.SetupGet(m => m.Type).Returns(ChallengeTypes.Http01);
            mockChallengeContext.Setup(m => m.Resource()).ReturnsAsync(new Challenge
            {
                Status = ChallengeStatus.Valid
            });

            mockOrderContext.Setup(m => m.Resource())
                .ReturnsAsync(new Order
                {
                    Identifiers = new List<Identifier>
                    {
                        new Identifier
                        {
                            Value = "test"
                        }
                    }
                });

            using var http = new HttpClient();
            var cert = await http.GetStringAsync("http://certes-ci.dymetis.com/cert-data").ConfigureAwait(false);
            mockOrderContext.Setup(m => m.Finalize(It.IsAny<byte[]>())).ReturnsAsync(new Order());
            mockOrderContext.Setup(m => m.Download()).ReturnsAsync(new CertificateChain(string.Join(Environment.NewLine,
                cert.Trim())));

            var sut = new LetsEncryptService(mockAcmeContext.Object, mockOptions.Object);

            var task = new TaskFactory().StartNew(() => sut.CreateCertificate(mockWebHost.Object));

            await Task.Delay(200).ConfigureAwait(false);

            sut.OnCertificateReady();

            await Task.Delay(200).ConfigureAwait(false);

            Assert.True(task.IsCompleted);
            Assert.False(task.IsFaulted);
            Assert.True(File.Exists(certesAccount.PfxPath));
        }

        [Fact]
        public async Task CreateCertificateAsync_should_verify_challenge_status()
        {
            var mockWebHost = new Mock<IWebHost>();
            mockWebHost.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOrderContext = new Mock<IOrderContext>();
            var mockAuthorizationContext = new Mock<IAuthorizationContext>();
            var mockChallengeContext = new Mock<IChallengeContext>();

            var mockOptions = new Mock<IOptions<CertesAccount>>();

            var certesAccount = new CertesAccount
            {
                Enable = true,
                ServerUrl = "http://test.com",
                AccountDer = Convert.ToBase64String(Convert.FromBase64String("test")),
                PfxPath = Guid.NewGuid().ToString()
            };

            mockOptions.SetupGet(m => m.Value).Returns(certesAccount);

            mockAcmeContext.Setup(m => m.NewOrder(It.IsAny<IList<string>>(), null, null))
                .ReturnsAsync(mockOrderContext.Object);

            mockOrderContext.Setup(m => m.Authorizations()).ReturnsAsync(new IAuthorizationContext[] { mockAuthorizationContext.Object });
            mockAuthorizationContext.Setup(m => m.Resource()).ReturnsAsync(new Authorization
            {
                Status = AuthorizationStatus.Pending
            });
            mockAuthorizationContext.Setup(m => m.Challenges()).ReturnsAsync(new IChallengeContext[] { mockChallengeContext.Object });
            mockChallengeContext.SetupGet(m => m.KeyAuthz).Returns("test");
            mockChallengeContext.SetupGet(m => m.Type).Returns(ChallengeTypes.Http01);
            mockChallengeContext.Setup(m => m.Resource()).ReturnsAsync(new Challenge
            {
                Status = ChallengeStatus.Invalid
            });

            var sut = new LetsEncryptService(mockAcmeContext.Object, mockOptions.Object);

            var task = new TaskFactory().StartNew(() => sut.CreateCertificate(mockWebHost.Object));

            await Task.Delay(200).ConfigureAwait(false);
            sut.OnCertificateReady();
            await Task.Delay(200).ConfigureAwait(false);

            Assert.True(task.IsCompleted);
            Assert.False(task.IsFaulted);
            Assert.False(File.Exists(certesAccount.PfxPath));
        }

        [Fact]
        public async Task CreateCertificateAsync_should_be_disablable()
        {
            var mockWebHost = new Mock<IWebHost>();
            mockWebHost.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOptions = new Mock<IOptions<CertesAccount>>();
            mockOptions.SetupGet(m => m.Value).Returns(new CertesAccount());

            var sut = new LetsEncryptService(mockAcmeContext.Object, mockOptions.Object);

            var task = new TaskFactory().StartNew(() => sut.CreateCertificate(mockWebHost.Object));

            await Task.Delay(200).ConfigureAwait(false);

            Assert.True(task.IsCompleted);
            Assert.False(task.IsFaulted);
        }

        [Fact]
        public void CreateCertificateAsync_should_stop_host_on_timeout()
        {
            var mockWebHost = new Mock<IWebHost>();
            mockWebHost.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOrderContext = new Mock<IOrderContext>();
            var mockAuthorizationContext = new Mock<IAuthorizationContext>();
            var mockChallengeContext = new Mock<IChallengeContext>();

            var mockOptions = new Mock<IOptions<CertesAccount>>();

            var certesAccount = new CertesAccount
            {
                Enable = true,
                Timeout = TimeSpan.FromMilliseconds(1),
                ServerUrl = "http://test.com",
                AccountDer = Convert.ToBase64String(Convert.FromBase64String("test")),
                PfxPath = Guid.NewGuid().ToString()
            };

            mockOptions.SetupGet(m => m.Value).Returns(certesAccount);

            mockAcmeContext.Setup(m => m.NewOrder(It.IsAny<IList<string>>(), null, null))
                .ReturnsAsync(mockOrderContext.Object);

            mockOrderContext.Setup(m => m.Authorizations()).ReturnsAsync(new IAuthorizationContext[] { mockAuthorizationContext.Object });
            mockAuthorizationContext.Setup(m => m.Resource()).ReturnsAsync(new Authorization
            {
                Status = AuthorizationStatus.Pending
            });
            mockAuthorizationContext.Setup(m => m.Challenges()).ReturnsAsync(new IChallengeContext[] { mockChallengeContext.Object });
            mockChallengeContext.SetupGet(m => m.KeyAuthz).Returns("test");
            mockChallengeContext.SetupGet(m => m.Type).Returns(ChallengeTypes.Http01);
            mockChallengeContext.Setup(m => m.Resource()).ReturnsAsync(new Challenge
            {
                Status = ChallengeStatus.Invalid
            });

            var sut = new LetsEncryptService(mockAcmeContext.Object, mockOptions.Object);

            sut.CreateCertificate(mockWebHost.Object);
            mockWebHost.Verify();
        }

        [Fact]
        public void CreateCertificateAsync_should_reset_envvars_on_timeout()
        {
            var mockWebHost = new Mock<IWebHost>();
            mockWebHost.Setup(m => m.StopAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();
            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOrderContext = new Mock<IOrderContext>();
            var mockAuthorizationContext = new Mock<IAuthorizationContext>();
            var mockChallengeContext = new Mock<IChallengeContext>();

            var mockOptions = new Mock<IOptions<CertesAccount>>();

            var certesAccount = new CertesAccount
            {
                Enable = true,
                Timeout = TimeSpan.FromMilliseconds(1),
                ServerUrl = "http://test.com",
                AccountDer = Convert.ToBase64String(Convert.FromBase64String("test")),
                PfxPath = Guid.NewGuid().ToString()
            };

            mockOptions.SetupGet(m => m.Value).Returns(certesAccount);

            mockAcmeContext.Setup(m => m.NewOrder(It.IsAny<IList<string>>(), null, null))
                .ReturnsAsync(mockOrderContext.Object);

            mockOrderContext.Setup(m => m.Authorizations()).ReturnsAsync(new IAuthorizationContext[] { mockAuthorizationContext.Object });
            mockAuthorizationContext.Setup(m => m.Resource()).ReturnsAsync(new Authorization
            {
                Status = AuthorizationStatus.Pending
            });
            mockAuthorizationContext.Setup(m => m.Challenges()).ReturnsAsync(new IChallengeContext[] { mockChallengeContext.Object });
            mockChallengeContext.SetupGet(m => m.KeyAuthz).Returns("test");
            mockChallengeContext.SetupGet(m => m.Type).Returns(ChallengeTypes.Http01);
            mockChallengeContext.Setup(m => m.Resource()).ReturnsAsync(new Challenge
            {
                Status = ChallengeStatus.Invalid
            });

            var sut = new LetsEncryptService(mockAcmeContext.Object, mockOptions.Object);

            Environment.SetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path", certesAccount.PfxPath);
            sut.CreateCertificate(mockWebHost.Object);
            mockWebHost.Verify();

            Assert.Null(Environment.GetEnvironmentVariable("ASPNETCORE_Kestrel__Certificates__Default__Path"));
        }
    }
}
