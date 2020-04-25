using Aguacongas.IdentityServer.Store.Entity;
using System;
using Auth = Aguacongas.AspNetCore.Authentication.EntityFramework;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class SchemeDefinition : Auth.SchemeDefinition, IAuditable
    {
        public string Id { get => Scheme; set => Scheme = value; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
