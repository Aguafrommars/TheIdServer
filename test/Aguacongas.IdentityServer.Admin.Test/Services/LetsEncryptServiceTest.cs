using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using Certes.Pkcs;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Services
{
    public class LetsEncryptServiceTest
    {
        [Fact]
        public async Task CreateCertificateAsync_should_create_certificate_file()
        {
            var mockAcmeContext = new Mock<IAcmeContext>();
            var mockOrderContext = new Mock<IOrderContext>();
            var mockAuthorizationContext = new Mock<IAuthorizationContext>();
            var mockChallengeContext = new Mock<IChallengeContext>();

            var mockOptions = new Mock<IOptions<CertesAccount>>();

            var certesAccount = new CertesAccount
            {
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

            await sut.CreateCertificateAsync();

            Assert.True(File.Exists(certesAccount.PfxPath));            
        }
    }
}
