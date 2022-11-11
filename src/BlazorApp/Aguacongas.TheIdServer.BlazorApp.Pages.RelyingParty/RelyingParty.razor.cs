// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using IdentityModel;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.RelyingParty
{
    public partial class RelyingParty
    {
        private readonly static IEnumerable<string> TOKENTYPELIST = new[]
        {
            "urn:oasis:names:tc:SAML:1.0:assertion",
            "urn:oasis:names:tc:SAML:2.0:assertion"
        };

        private readonly static IEnumerable<string> SIGNATUREALGORITHMLIST = new[]
        {
            SecurityAlgorithms.RsaSha256Signature,
            SecurityAlgorithms.RsaSha384Signature,
            SecurityAlgorithms.RsaSha512Signature
        };

        private readonly static IEnumerable<string> DIGESTLIST = new[]
        {
            SecurityAlgorithms.Sha256Digest,
            SecurityAlgorithms.Sha384Digest,
            SecurityAlgorithms.Sha512Digest
        };

        private readonly static IEnumerable<string> NAMEIDENTIFIERFORMATLIST = new[]
        {
            "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
            "urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress",
            "urn:oasis:names:tc:SAML:2.0:nameid-format:encrypted",
            "urn:oasis:names:tc:SAML:2.0:nameid-format:entity",
            "urn:oasis:names:tc:SAML:2.0:nameid-format:kerberos",
            "urn:oasis:names:tc:SAML:2.0:nameid-format:persistent",
            "urn:oasis:names:tc:SAML:2.0:nameid-format:transient",
            "urn:oasis:names:tc:SAML:1.1:nameid-format:WindowsDomainQualifiedName",
            "urn:oasis:names:tc:SAML:1.1:nameid-format:X509SubjectName"
        };

        private IEnumerable<string> _thumbprint;

        protected override string Expand => $"{nameof(Entity.RelyingParty.ClaimMappings)}";

        protected override bool NonEditable => false;

        protected override string BackUrl => "relyingparties";

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            if (Model.EncryptionCertificate != null)
            {
                await SetThrumprint(Model.EncryptionCertificate).ConfigureAwait(false);
            }
        }

        protected override Task<Entity.RelyingParty> Create()
        {
            var claimMappingList = new List<Entity.RelyingPartyClaimMapping>
                {
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.Name,
                        ToClaimType = ClaimTypes.Name
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.Subject,
                        ToClaimType = ClaimTypes.NameIdentifier
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.Email,
                        ToClaimType = ClaimTypes.Email
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.GivenName,
                        ToClaimType = ClaimTypes.GivenName
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.FamilyName,
                        ToClaimType = ClaimTypes.Surname
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.BirthDate,
                        ToClaimType = ClaimTypes.DateOfBirth
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.WebSite,
                        ToClaimType = ClaimTypes.Webpage
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.Gender,
                        ToClaimType = ClaimTypes.Gender
                    },
                    new Entity.RelyingPartyClaimMapping
                    {
                        FromClaimType = JwtClaimTypes.Role,
                        ToClaimType = ClaimTypes.Role
                    }
                };

            foreach(var item in claimMappingList)
            {
                HandleModificationState.EntityCreated(item);
            }

            return Task.FromResult(new Entity.RelyingParty
            {
                TokenType = TOKENTYPELIST.First(),
                DigestAlgorithm = DIGESTLIST.First(),
                SamlNameIdentifierFormat = NAMEIDENTIFIERFORMATLIST.First(),
                SignatureAlgorithm = SIGNATUREALGORITHMLIST.First(),
                ClaimMappings = claimMappingList
            });
        }

        protected override void RemoveNavigationProperty<TEntity>(TEntity entity)
        {
            if (entity is Entity.RelyingParty identity)
            {
                identity.ClaimMappings = null;
            }
        }

        protected override void SanetizeEntityToSaved<TEntity>(TEntity entity)
        {
            if (entity is Entity.RelyingParty identity)
            {
                Model.Id = identity.Id;
            }
            if (entity is Entity.RelyingPartyClaimMapping subEntity)
            {
                subEntity.RelyingPartyId = Model.Id;
            }
        }
       
        private async Task SetCertificateAsync(InputFileChangeEventArgs e)
        {
            using var stream = e.File.OpenReadStream();
            var size = e.File.Size;
            var content = new byte[size];

            await stream.ReadAsync(content.AsMemory(0, (int)size)).ConfigureAwait(false);
            await SetThrumprint(content).ConfigureAwait(false);
        }

        private async Task SetThrumprint(byte[] content)
        {
            var client = _factory.CreateClient("oidc");
            using var putContent = new ByteArrayContent(content);
            try
            {
                var response = await client.PutAsync("api/certificate", putContent).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    _thumbprint = new[] { Localizer["Invalid file"].Value };
                    await InvokeAsync(StateHasChanged).ConfigureAwait(false);
                    return;
                }

                Model.EncryptionCertificate = content;
                _thumbprint = await JsonSerializer.DeserializeAsync<IEnumerable<string>>(await response.Content.ReadAsStreamAsync()).ConfigureAwait(false);
                HandleModificationState.EntityUpdated(Model);
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
            catch (CryptographicException)
            {
                _thumbprint = new[] { Localizer["Invalid file"].Value };
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            }
        }

        private void RemoveCertificate()
        {
            _thumbprint = null;
            Model.EncryptionCertificate = null;
            HandleModificationState.EntityUpdated(Model);
            StateHasChanged();
        }

        private Entity.RelyingPartyClaimMapping CreateMapping()
        {
            return new Entity.RelyingPartyClaimMapping
            {
                RelyingPartyId = Model.Id
            };
        }
    }
}
