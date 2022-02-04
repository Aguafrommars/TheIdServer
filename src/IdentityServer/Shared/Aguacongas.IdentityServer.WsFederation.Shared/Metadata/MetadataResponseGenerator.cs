// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
#else
using IdentityServer4.Extensions;
using IdentityServer4.Stores;
#endif
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// Generate Ws-Federation metadata document
    /// </summary>
    public class MetadataResponseGenerator : IMetadataResponseGenerator
    {
        private readonly ISigningCredentialStore _keys;

#if DUENDE
        private readonly IIssuerNameService _issuerNameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResponseGenerator"/> class.
        /// </summary>
        /// <param name="issuerNameService">The <see cref="IIssuerNameService"/>.</param>
        /// <param name="keys">The keys.</param>
        public MetadataResponseGenerator(IIssuerNameService issuerNameService, ISigningCredentialStore keys)
        {
            _keys = keys;
            _issuerNameService = issuerNameService;
        }
#else
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResponseGenerator"/> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="keys">The keys.</param>
        public MetadataResponseGenerator(IHttpContextAccessor contextAccessor, ISigningCredentialStore keys)
        {
            _keys = keys;
            _contextAccessor = contextAccessor;
        }
#endif
        /// <summary>
        /// Generates the asynchronous.
        /// </summary>
        /// <param name="wsfedEndpoint">The wsfed endpoint.</param>
        /// <returns></returns>
        public async Task<WsFederationConfiguration> GenerateAsync(string wsfedEndpoint)
        {
            var credentials = await _keys.GetSigningCredentialsAsync().ConfigureAwait(false);
            var key = credentials.Key;
            var keyInfo = new KeyInfo(key.GetX509Certificate(_keys));
#if DUENDE
            var issuer = await _issuerNameService.GetCurrentAsync().ConfigureAwait(false);
#else
            var issuer = _contextAccessor.HttpContext.GetIdentityServerIssuerUri();
#endif
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest);
            var config = new WsFederationConfiguration()
            {
                Issuer = issuer,
                TokenEndpoint = wsfedEndpoint,
                SigningCredentials = signingCredentials,
            };
            config.SigningKeys.Add(key);
            config.KeyInfos.Add(keyInfo);

            return config;
        }
    }
}
