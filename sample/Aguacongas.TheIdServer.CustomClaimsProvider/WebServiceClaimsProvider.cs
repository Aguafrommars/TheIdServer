// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Abstractions;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.CustomClaimsProviders
{
    public class WebServiceClaimsProvider : IProvideClaims
    {
        private readonly HttpClient _httpClient;

        public WebServiceClaimsProvider(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }
        public async Task<IEnumerable<Claim>> ProvideClaims(ClaimsPrincipal subject, Client client, string caller, Resource resource)
        {
            var response =  await _httpClient.GetAsync($"/claims?subject={subject.Identity.Name}&client={client.ClientName}").ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            var entities = await JsonSerializer.DeserializeAsync<IEnumerable<SerializedClaim>>(content).ConfigureAwait(false);
            return entities.Select(e => e.ToClaim());
        }

        [SuppressMessage("Minor Code Smell", "S3459:Unassigned members should be removed", Justification = "Deserialized")]
        class SerializedClaim
        {
            
            public string Type { get; set; }

            public string Value { get; set; }

            public Claim ToClaim()
                => new Claim(Type, Value);
        }
    }
}
