using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.Schemas;
using ITfoxtec.Identity.Saml2.MvcCore;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens.Saml2;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using Duende.IdentityServer.Stores;
#if DEBUG
using System.Diagnostics;
#endif

namespace Aguacongas.IdentityServer.Saml2p.Duende
{
    [AllowAnonymous]
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly Saml2Configuration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        // List of Artifacts for test purposes.
        private static ConcurrentDictionary<string, Saml2AuthnResponse> artifactSaml2AuthnResponseCache = new ConcurrentDictionary<string, Saml2AuthnResponse>();

        public AuthController(IClientStore clientStore, Saml2Configuration config, IHttpClientFactory httpClientFactory)
        {
            this._config = config;
            this._httpClientFactory = httpClientFactory;
        }

        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            var requestBinding = new Saml2RedirectBinding();
            var relyingParty = await ValidateRelyingParty(ReadRelyingPartyFromLoginRequest(requestBinding));

            var saml2AuthnRequest = new Saml2AuthnRequest(GetRpSaml2Configuration());
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2AuthnRequest);

                // ****  Handle user login e.g. in GUI ****
                // Test user with session index and claims
                var sessionIndex = Guid.NewGuid().ToString();
                var claims = CreateTestUserClaims(saml2AuthnRequest.Subject?.NameID?.ID);

                return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Success, requestBinding.RelayState, relyingParty, sessionIndex, claims);
            }
            catch (Exception exc)
            {
#if DEBUG
                Debug.WriteLine($"Saml 2.0 Authn Request error: {exc.ToString()}\nSaml Auth Request: '{saml2AuthnRequest.XmlDocument?.OuterXml}'\nQuery String: {Request.QueryString}");
#endif
                return LoginResponse(saml2AuthnRequest.Id, Saml2StatusCodes.Responder, requestBinding.RelayState, relyingParty);
            }
        }
        
        [Route("Artifact")]
        public async Task<IActionResult> Artifact()
        {
            try
            {
                var soapEnvelope = new Saml2SoapEnvelope();

                var httpRequest = await Request.ToGenericHttpRequestAsync(readBodyAsString: true);
                var relyingParty = await ValidateRelyingParty(ReadRelyingPartyFromSoapEnvelopeRequest(httpRequest, soapEnvelope));

                var saml2ArtifactResolve = new Saml2ArtifactResolve(GetRpSaml2Configuration(relyingParty));
                soapEnvelope.Unbind(httpRequest, saml2ArtifactResolve);

                if (!artifactSaml2AuthnResponseCache.Remove(saml2ArtifactResolve.Artifact, out Saml2AuthnResponse saml2AuthnResponse))
                {
                    throw new Exception($"Saml2AuthnResponse not found by Artifact '{saml2ArtifactResolve.Artifact}' in the cache.");
                }

                var saml2ArtifactResponse = new Saml2ArtifactResponse(_config, saml2AuthnResponse)
                {
                    InResponseTo = saml2ArtifactResolve.Id
                };
                soapEnvelope.Bind(saml2ArtifactResponse);
                return soapEnvelope.ToActionResult();
            }
            catch (Exception exc)
            {
#if DEBUG
                Debug.WriteLine($"SPSsoDescriptor error: {exc.ToString()}");
#endif
                throw;
            }
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var requestBinding = new Saml2PostBinding();
            var relyingParty = await ValidateRelyingParty(ReadRelyingPartyFromLogoutRequest(requestBinding));

            var saml2LogoutRequest = new Saml2LogoutRequest(GetRpSaml2Configuration(relyingParty));
            try
            {
                requestBinding.Unbind(Request.ToGenericHttpRequest(), saml2LogoutRequest);

                // **** Delete user session ****

                return LogoutResponse(saml2LogoutRequest.Id, Saml2StatusCodes.Success, requestBinding.RelayState, saml2LogoutRequest.SessionIndex, relyingParty);
            }
            catch (Exception exc)
            {
#if DEBUG
                Debug.WriteLine($"Saml 2.0 Logout Request error: {exc.ToString()}\nSaml Logout Request: '{saml2LogoutRequest.XmlDocument?.OuterXml}'");
#endif
                return LogoutResponse(saml2LogoutRequest.Id, Saml2StatusCodes.Responder, requestBinding.RelayState, saml2LogoutRequest.SessionIndex, relyingParty);
            }
        }

        private string ReadRelyingPartyFromLoginRequest<T>(Saml2Binding<T> binding)
        {
            return binding.ReadSamlRequest(Request.ToGenericHttpRequest(), new Saml2AuthnRequest(GetRpSaml2Configuration()))?.Issuer;
        }

        private string ReadRelyingPartyFromLogoutRequest<T>(Saml2Binding<T> binding)
        {
            return binding.ReadSamlRequest(Request.ToGenericHttpRequest(), new Saml2LogoutRequest(GetRpSaml2Configuration()))?.Issuer;
        }

        private string ReadRelyingPartyFromSoapEnvelopeRequest<T>(ITfoxtec.Identity.Saml2.Http.HttpRequest httpRequest, Saml2Binding<T> binding)
        {
            return binding.ReadSamlRequest(httpRequest, new Saml2ArtifactResolve(GetRpSaml2Configuration()))?.Issuer;
        }

        private IActionResult LoginResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty relyingParty, string sessionIndex = null, IEnumerable<Claim> claims = null)
        {
            if (relyingParty.UseAcsArtifact)
            {
                return LoginArtifactResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, claims);
            }
            else
            {
                return LoginPostResponse(inResponseTo, status, relayState, relyingParty, sessionIndex, claims);
            }
        }

        private IActionResult LoginPostResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty relyingParty, string sessionIndex = null, IEnumerable<Claim> claims = null)
        {
            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = relayState;

            var saml2AuthnResponse = new Saml2AuthnResponse(GetRpSaml2Configuration(relyingParty))
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination = relyingParty.AcsDestination,
            };
            if (status == Saml2StatusCodes.Success && claims != null)
            {
                saml2AuthnResponse.SessionIndex = sessionIndex;

                var claimsIdentity = new ClaimsIdentity(claims);
                saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
                //saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single());
                saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

                var token = saml2AuthnResponse.CreateSecurityToken(relyingParty.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
            }

            return responsebinding.Bind(saml2AuthnResponse).ToActionResult();
        }

        private IActionResult LoginArtifactResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, RelyingParty relyingParty, string sessionIndex = null, IEnumerable<Claim> claims = null)
        {
            var responsebinding = new Saml2ArtifactBinding();
            responsebinding.RelayState = relayState;

            var saml2ArtifactResolve = new Saml2ArtifactResolve(GetRpSaml2Configuration(relyingParty))
            {
                Destination = relyingParty.AcsDestination
            };
            responsebinding.Bind(saml2ArtifactResolve);

            var saml2AuthnResponse = new Saml2AuthnResponse(GetRpSaml2Configuration(relyingParty))
            {
                InResponseTo = inResponseTo,
                Status = status
            };
            if (status == Saml2StatusCodes.Success && claims != null)
            {
                saml2AuthnResponse.SessionIndex = sessionIndex;

                var claimsIdentity = new ClaimsIdentity(claims);
                saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single(), NameIdentifierFormats.Persistent);
                //saml2AuthnResponse.NameId = new Saml2NameIdentifier(claimsIdentity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).Single());
                saml2AuthnResponse.ClaimsIdentity = claimsIdentity;

                var token = saml2AuthnResponse.CreateSecurityToken(relyingParty.Issuer, subjectConfirmationLifetime: 5, issuedTokenLifetime: 60);
            }
            artifactSaml2AuthnResponseCache[saml2ArtifactResolve.Artifact] = saml2AuthnResponse;

            return responsebinding.ToActionResult();
        }

        private IActionResult LogoutResponse(Saml2Id inResponseTo, Saml2StatusCodes status, string relayState, string sessionIndex, RelyingParty relyingParty)
        {
            var responsebinding = new Saml2PostBinding();
            responsebinding.RelayState = relayState;

            var saml2LogoutResponse = new Saml2LogoutResponse(GetRpSaml2Configuration(relyingParty))
            {
                InResponseTo = inResponseTo,
                Status = status,
                Destination = relyingParty.SingleLogoutDestination,
                SessionIndex = sessionIndex
            };

            return responsebinding.Bind(saml2LogoutResponse).ToActionResult();
        }

        private Saml2Configuration GetRpSaml2Configuration(RelyingParty relyingParty = null)
        {
            var rpConfig = new Saml2Configuration()
            {
                Issuer = _config.Issuer,
                SingleSignOnDestination = _config.SingleSignOnDestination,
                SingleLogoutDestination = _config.SingleLogoutDestination,
                ArtifactResolutionService = _config.ArtifactResolutionService,
                SigningCertificate = _config.SigningCertificate,
                SignatureAlgorithm = _config.SignatureAlgorithm,
                CertificateValidationMode = _config.CertificateValidationMode,
                RevocationMode = _config.RevocationMode
            };

            rpConfig.AllowedAudienceUris.AddRange(_config.AllowedAudienceUris);

            if (relyingParty != null) 
            {
                rpConfig.SignatureValidationCertificates.Add(relyingParty.SignatureValidationCertificate);
                rpConfig.EncryptionCertificate = relyingParty.EncryptionCertificate;
            }

            return rpConfig;
        }

        private async Task<RelyingParty> ValidateRelyingParty(string issuer)
        {
            using var cancellationTokenSource = new CancellationTokenSource(3 * 1000); // Cancel after 3 seconds.
            await Task.WhenAll(settings.RelyingParties.Select(rp => LoadRelyingPartyAsync(rp, cancellationTokenSource)));

            return settings.RelyingParties.Where(rp => rp.Issuer != null && rp.Issuer.Equals(issuer, StringComparison.InvariantCultureIgnoreCase)).Single();
        }

        private async Task LoadRelyingPartyAsync(RelyingParty rp, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                // Load RP if not already loaded.
                if (string.IsNullOrEmpty(rp.Issuer))
                {
                    var entityDescriptor = new EntityDescriptor();
                    await entityDescriptor.ReadSPSsoDescriptorFromUrlAsync(_httpClientFactory, new Uri(rp.Metadata), cancellationTokenSource.Token);
                    if (entityDescriptor.SPSsoDescriptor != null)
                    {
                        rp.Issuer = entityDescriptor.EntityId;
                        rp.AcsDestination = entityDescriptor.SPSsoDescriptor.AssertionConsumerServices.Where(a => a.IsDefault).OrderBy(a => a.Index).First().Location;
                        var singleLogoutService = entityDescriptor.SPSsoDescriptor.SingleLogoutServices.First();
                        rp.SingleLogoutDestination = singleLogoutService.ResponseLocation ?? singleLogoutService.Location;
                        rp.SignatureValidationCertificate = entityDescriptor.SPSsoDescriptor.SigningCertificates.First();
                    }
                    else
                    {
                        throw new Exception($"SPSsoDescriptor not loaded from metadata '{rp.Metadata}'.");
                    }
                }
            }
            catch (Exception exc)
            {
                //log error
#if DEBUG
                Debug.WriteLine($"SPSsoDescriptor error: {exc.ToString()}");
#endif
            }
        }

        private IEnumerable<Claim> CreateTestUserClaims(string selectedNameID)
        {
            var userId = selectedNameID ?? "12345";
            yield return new Claim(ClaimTypes.NameIdentifier, userId);
            yield return new Claim(ClaimTypes.Upn, $"{userId}@email.test");
            yield return new Claim(ClaimTypes.Email, $"{userId}@someemail.test");
        }
    }
}        