// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
#else
using IdentityServer4.Extensions;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
#endif
using Microsoft.Extensions.Options;
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
        private readonly IOptions<WsFederationOptions> _options;

#if DUENDE
        private readonly IIssuerNameService _issuerNameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResponseGenerator"/> class.
        /// </summary>
        /// <param name="issuerNameService">The <see cref="IIssuerNameService"/>.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">WS-Federation options</param>
        public MetadataResponseGenerator(IIssuerNameService issuerNameService, ISigningCredentialStore keys, IOptions<WsFederationOptions> options)
        {
            _keys = keys;
            _issuerNameService = issuerNameService;
            _options = options;
        }
#else
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetadataResponseGenerator"/> class.
        /// </summary>
        /// <param name="contextAccessor">The context accessor.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="options">WS-Federation options</param>
        public MetadataResponseGenerator(IHttpContextAccessor contextAccessor, ISigningCredentialStore keys, IOptions<WsFederationOptions> options)
        {
            _keys = keys;
            _contextAccessor = contextAccessor;
            _options = options;
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
            var settings = _options.Value;
            config.ClaimTypesOffered = settings.ClaimTypesOffered;
            config.ClaimTypesRequested = settings.ClaimTypesRequested;
            config.TokenTypesOffered = settings.TokenTypesOffered;
            return config;
        }
    }
}
