// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
#if DUENDE
using Duende.IdentityServer.Extensions;
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

            var issuer = _contextAccessor.HttpContext.GetIdentityServerIssuerUri();
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
