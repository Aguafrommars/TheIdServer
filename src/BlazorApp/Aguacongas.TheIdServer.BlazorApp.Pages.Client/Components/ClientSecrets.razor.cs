// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System;
using System.Collections.Generic;
using System.Linq;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.Client.Components
{
    public partial class ClientSecrets
    {
        private IEnumerable<Entity.ClientSecret> Secrets => Collection.Where(s => s.Id == null || (s.Description != null && s.Description.Contains(HandleModificationState.FilterTerm)) || (s.Type != null && s.Type.Contains(HandleModificationState.FilterTerm)));

        private static void GenerateSecret(Entity.ClientSecret secret)
        {
            secret.Value = Guid.NewGuid().ToString();
        }
    }
}
