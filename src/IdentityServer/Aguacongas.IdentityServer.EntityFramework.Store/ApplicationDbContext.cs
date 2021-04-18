// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Aguacongas.TheIdServer.Data
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        //
        // Summary:
        //     Gets or sets the Microsoft.EntityFrameworkCore.DbSet`1 of User claims.
        public virtual DbSet<UserClaim> UserClaims { get; set; }
        //
        // Summary:
        //     Gets or sets the Microsoft.EntityFrameworkCore.DbSet`1 of User logins.
        public virtual DbSet<UserLogin> UserLogins { get; set; }
        //
        // Summary:
        //     Gets or sets the Microsoft.EntityFrameworkCore.DbSet`1 of User tokens.
        public virtual DbSet<UserToken> UserTokens { get; set; }

        public virtual DbSet<UserRole> UserRoles { get; set; }
        //
        // Summary:
        //     Gets or sets the Microsoft.EntityFrameworkCore.DbSet`1 of roles.
        public virtual DbSet<Role> Roles { get; set; }
        //
        // Summary:
        //     Gets or sets the Microsoft.EntityFrameworkCore.DbSet`1 of role claims.
        public virtual DbSet<RoleClaim> RoleClaims { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in GetType().GetProperties().Where(p => p.PropertyType.ImplementsGenericInterface(typeof(IQueryable<>))))
            {
                var entityType = property.PropertyType.GetGenericArguments()[0];
                modelBuilder.Entity(entityType).ToTable($"AspNet{property.Name}");
            }
            modelBuilder.Entity<User>().Ignore(u => u.Password);
            modelBuilder.Entity<User>().HasIndex(u => u.NormalizedEmail)
                .HasDatabaseName("EmailIndex")
                .IsUnique(false);
            modelBuilder.Entity<User>().HasIndex(u => u.NormalizedUserName)
                .HasDatabaseName("UserNameIndex")
                .IsUnique();
            modelBuilder.Entity<User>().Property(u => u.NormalizedUserName).HasMaxLength(256);
            modelBuilder.Entity<User>().Property(u => u.UserName).HasMaxLength(256);
            modelBuilder.Entity<User>().Property(u => u.NormalizedEmail).HasMaxLength(256);
            modelBuilder.Entity<User>().Property(u => u.Email).HasMaxLength(256);

            modelBuilder.Entity<Role>().HasIndex(r => r.NormalizedName)
                .HasDatabaseName("RoleNameIndex")
                .IsUnique();
            modelBuilder.Entity<Role>().Property(r => r.NormalizedName).HasMaxLength(256);
            modelBuilder.Entity<Role>().Property(r => r.Name).HasMaxLength(256);

            modelBuilder.Entity<UserClaim>().Property(c => c.Id).HasConversion<int>();

            modelBuilder.Entity<UserLogin>().HasKey(l => new { l.LoginProvider, l.ProviderKey });
                
            modelBuilder.Entity<UserRole>().HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<UserToken>().HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

            modelBuilder.Entity<RoleClaim>().Property(c => c.Id).HasConversion<int>();

            base.OnModelCreating(modelBuilder);
        }

    }
}
