// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Components.UserComponents
{
    public partial class UserConsents
    {
        private IEnumerable<Entity.UserConsent> Consents => Collection.Where(c => c.ClientId.Contains(HandleModificationState.FilterTerm));

        public static IEnumerable<string> GetScopes(Entity.UserConsent consent)
        {
            var scopes = JsonSerializer.Deserialize<Data>(consent.Data)?.Scopes;
            if (scopes == null)
            {
                scopes = new List<string>();
            }
            return scopes;
        }

        class Data
        {
#pragma warning disable S3459 // Unassigned members should be removed
#pragma warning disable S1144 // Unused private types or members should be removed
            public IEnumerable<string> Scopes { get; set; }
#pragma warning restore S1144 // Unused private types or members should be removed
#pragma warning restore S3459 // Unassigned members should be removed
        }
    }
}
