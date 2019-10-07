using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.EntityFramework.Store
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Identity> Identities { get; set; }

        public DbSet<IdentityClaim> IdentityClaims { get; set; }

        public DbSet<IdentityProperty> IdentityProperties { get; set; }
    }
}
