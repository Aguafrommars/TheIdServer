using Aguacongas.IdentityServer.Admin.Models;
using Certes;
using Certes.Acme;
using Certes.Acme.Resource;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Manage SSL certificates.
    /// </summary>
    public class LetsEncryptService
    {
        private const string ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PATH = "ASPNETCORE_Kestrel__Certificates__Default__Path";
        private const string ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PASSWORD = "ASPNETCORE_Kestrel__Certificates__Default__Password";

        private readonly IAcmeContext _context;
        private readonly IOptions<CertesAccount> _options;
        private IChallengeContext _challengeContext;
        private IOrderContext _orderContext;

        /// <summary>
        /// Gets the key authz.
        /// </summary>
        /// <value>
        /// The key authz.
        /// </value>
        public string KeyAuthz { get; private set; }

        /// <summary>
        /// Gets the on certificate ready action.
        /// </summary>
        /// <value>
        /// The on certificate ready.
        /// </value>
        public Action OnCertificateReady { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LetsEncryptService"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="options">The options.</param>
        /// <exception cref="ArgumentNullException">
        /// context
        /// or
        /// options
        /// </exception>
        public LetsEncryptService(IAcmeContext context, IOptions<CertesAccount> options)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Creates the certificate.
        /// </summary>
        /// <param name="host">The host.</param>
        public IWebHost CreateCertificate(IWebHost host)
        {
            var settings = _options.Value;
            if (!settings.Enable)
            {
                return host;
            }

            using (host)
            {
                host.Start();
                var resetEvent = new ManualResetEvent(false);
                OnCertificateReady = async () =>
                {
                    await CreateCredentialFileAsync().ConfigureAwait(false);
                    resetEvent.Set();
                };
                CreateNewAuthaurizationKeyAsync().GetAwaiter().GetResult();

                if (!resetEvent.WaitOne(settings.Timeout))
                {
                    var hasHttpsCertificate = Environment.GetEnvironmentVariable(ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PATH) == settings.PfxPath;
                    if (hasHttpsCertificate)
                    {
                        Environment.SetEnvironmentVariable(ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PASSWORD, null);
                        Environment.SetEnvironmentVariable(ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PATH, null);
                    }
                }

                host.StopAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();
            }

            return null;
        }

        private async Task CreateNewAuthaurizationKeyAsync()
        {
            var settings = _options.Value;
            _orderContext = await _context.NewOrder(new[] { settings.Domain })
                .ConfigureAwait(false);
            var authorizations = await _orderContext.Authorizations().ConfigureAwait(false);
            foreach(var authorizationContext in authorizations)
            {
                var authorization = await authorizationContext.Resource().ConfigureAwait(false);
                if (authorization.Status == AuthorizationStatus.Pending)
                {
                    _challengeContext = await authorizationContext.Http().ConfigureAwait(false);
                    KeyAuthz = _challengeContext.KeyAuthz;
                    await _challengeContext.Validate().ConfigureAwait(false);
                    break;
                }
            }            
        }

        private async Task CreateCredentialFileAsync()
        {            
            var challenge = await _challengeContext.Resource().ConfigureAwait(false);

            if (challenge.Status == ChallengeStatus.Invalid)
            {
                return;
            }

            var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
            var certificationRequestBuilder = await _orderContext.CreateCsr(privateKey).ConfigureAwait(false);
            var csr = certificationRequestBuilder.Generate();

            await _orderContext.Finalize(csr).ConfigureAwait(false);

            var certificateChain = await _orderContext.Download().ConfigureAwait(false);

            var pfxBuilder = certificateChain.ToPfx(privateKey);

            var pfxName = string.Format(CultureInfo.InvariantCulture, "[certes] {0:yyyyMMddhhmmss}", DateTime.UtcNow);

            var settings = _options.Value;
            var pfx = pfxBuilder.Build(pfxName, settings.PfxPassword);

            await File.WriteAllBytesAsync(settings.PfxPath, pfx).ConfigureAwait(false);

            Environment.SetEnvironmentVariable(ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PASSWORD, settings.PfxPassword);
            Environment.SetEnvironmentVariable(ASPNETCORE_KESTREL_CERTIFICATES_DEFAULT_PATH, settings.PfxPath);
        }
    }
}
