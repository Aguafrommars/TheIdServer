// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using System;
using System.Collections.Generic;
using Auth = Aguacongas.AspNetCore.Authentication.EntityFramework;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class SchemeDefinition : Auth.SchemeDefinition, IAuditable
    {
        public string Id { get => Scheme; set => Scheme = value; }

        public bool StoreClaims { get; set; }

        public bool MapDefaultOutboundClaimType { get; set; }

        public virtual ICollection<ExternalClaimTransformation> ClaimTransformations { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }        
    }
}
