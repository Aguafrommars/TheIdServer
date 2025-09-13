// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using System.Collections.Generic;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class Role : Entity.Role, ICloneable<Role>
    {
        public ICollection<Entity.RoleClaim> Claims { get; set; }

        public new Role Clone()
        {
            return MemberwiseClone() as Role;
        }

        public static Role FromEntity(Entity.Role role)
        {
            return new Role
            {
                Id = role.Id,
                Name = role.Name,
                Claims = role.RoleClaims
            };
        }
    }
}
