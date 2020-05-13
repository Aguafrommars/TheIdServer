using Aguacongas.IdentityServer.Admin.Models;
using Certes;
using Certes.Acme.Resource;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// Manage SSL certificates.
    /// </summary>
    public class LetsEncryptService
    {
        private readonly IAcmeContext _context;
        private readonly IOptions<CertesAccount> _options;

        /// <summary>
        /// Gets the key authz.
        /// </summary>
        /// <value>
        /// The key authz.
        /// </value>
        public string KeyAuthz { get; private set; }

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
        public async Task CreateCertificateAsync()
        {
            var settings = _options.Value;
            var orderContext = await _context.NewOrder(new[] { settings.Domain })
                .ConfigureAwait(false);
            var authorizations = await orderContext.Authorizations().ConfigureAwait(false);
            foreach(var authorizationContext in authorizations)
            {
                var authorization = await authorizationContext.Resource().ConfigureAwait(false);
                if (authorization.Status == AuthorizationStatus.Pending)
                {
                    var challengeContext = await authorizationContext.Http().ConfigureAwait(false);
                    KeyAuthz = challengeContext.KeyAuthz;
                    await challengeContext.Validate().ConfigureAwait(false);
                    Challenge challenge;
                    do
                    {
                        await Task.Delay(200).ConfigureAwait(false);
                        challenge = await challengeContext.Resource().ConfigureAwait(false);
                    } while (challenge.Status != ChallengeStatus.Invalid && challenge.Status != ChallengeStatus.Valid);

                    if (challenge.Status == ChallengeStatus.Invalid)
                    {
                        return;
                    }

                    var privateKey = KeyFactory.NewKey(KeyAlgorithm.ES256);
                    var certificationRequestBuilder = await orderContext.CreateCsr(privateKey).ConfigureAwait(false);
                    var csr = certificationRequestBuilder.Generate();

                    await orderContext.Finalize(csr).ConfigureAwait(false);

                    var certificateChain = await orderContext.Download().ConfigureAwait(false);

                    var pfxBuilder = certificateChain.ToPfx(privateKey);

                    var pfxName = string.Format(CultureInfo.InvariantCulture, "[certes] {0:yyyyMMddhhmmss}", DateTime.UtcNow);
                    var pfx = pfxBuilder.Build(pfxName, settings.PfxPassword);

                    await File.WriteAllBytesAsync(settings.PfxPath, pfx).ConfigureAwait(false);

                    break;
                }
            }            
        }
    }
}
