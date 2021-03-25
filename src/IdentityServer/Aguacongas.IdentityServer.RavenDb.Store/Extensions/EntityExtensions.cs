// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.RavenDb.Store;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aguacongas.IdentityServer.Store
{
    public static class EntityExtensions
    {
        public static Entity.RoleClaim ToEntity(this IdentityRoleClaim<string> claim)
        {
            return new Entity.RoleClaim
            {
                Id = $"{claim.RoleId}@{claim.Id}",
                RoleId = claim.RoleId,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
        }

        public static Entity.UserClaim ToEntity(this UserClaim claim)
        {
            return new Entity.UserClaim
            {
                Id = $"{claim.UserId}@{claim.Id}",
                UserId = claim.UserId,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
                Issuer = claim.Issuer,
                OriginalType = claim.OriginalType
            };
        }

        public static Entity.UserRole ToEntity(this IdentityUserRole<string> userRole, IdentityRole role)
        {
            return new Entity.UserRole
            {
                Id = $"{role.NormalizedName}@{userRole.UserId}",
                RoleId = userRole.RoleId,
                UserId = userRole.UserId
            };
        }

        public static Entity.UserToken ToEntity(this IdentityUserToken<string> token)
        {
            return new Entity.UserToken
            {
                Id = $"{token.UserId}@{token.LoginProvider}@{token.Name}",
                UserId = token.UserId,
                LoginProvider = token.LoginProvider,
                Name = token.Name,
                Value = token.Value
            };
        }

        public static Entity.UserLogin ToEntity(this IdentityUserLogin<string> login)
        {
            return new Entity.UserLogin
            {
                Id = $"{login.UserId}@{login.LoginProvider}@{login.ProviderKey}",
                UserId = login.UserId,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey,
                ProviderDisplayName = login.ProviderDisplayName
            };
        }

        public static Entity.Role ToEntity<TRole>(this TRole role, ICollection<Entity.RoleClaim> claims = null) where TRole : IdentityRole
        {
            if (role == null)
            {
                return null;
            }
            return new Entity.Role
            {
                Id = role.Id,
                Name = role.Name,
                ConcurrencyStamp = role.ConcurrencyStamp,
                NormalizedName = role.NormalizedName,
                RoleClaims = claims
            };
        }

        public static TUser ToUser<TUser>(this Entity.User entity) where TUser : IdentityUser, new()
        {
            return new TUser
            {
                Id = entity.Id,
                AccessFailedCount = entity.AccessFailedCount,
                Email = entity.Email,
                EmailConfirmed = entity.EmailConfirmed,
                LockoutEnabled = entity.LockoutEnabled,
                LockoutEnd = entity.LockoutEnd,
                PhoneNumber = entity.PhoneNumber,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                TwoFactorEnabled = entity.TwoFactorEnabled,
                UserName = entity.UserName,
                ConcurrencyStamp = entity.ConcurrencyStamp,
                NormalizedEmail = entity.NormalizedEmail,
                NormalizedUserName = entity.NormalizedUserName,
                PasswordHash = entity.PasswordHash,
                SecurityStamp = entity.SecurityStamp
            };
        }

        public static TRole ToRole<TRole>(this Entity.Role entity) where TRole : IdentityRole, new()
        {
            return new TRole
            {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        public static IdentityRoleClaim<string> ToRoleClaim(this Entity.RoleClaim claim)
        {
            if (!int.TryParse(claim.Id, out int id))
            {
                id = 0;
            }

            return new IdentityRoleClaim<string>
            {
                Id = id,
                RoleId = claim.RoleId,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
        }

        public static UserClaim ToUserClaim(this Entity.UserClaim claim)
        {
            if (!int.TryParse(claim.Id, out int id))
            {
                id = 0;
            }

            return new UserClaim
            {
                Id = id,
                UserId = claim.UserId,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
                Issuer = claim.Issuer,
                OriginalType = claim.OriginalType
            };
        }

        public static Entity.User ToUserEntity<TUser>(this TUser user, ICollection<Entity.UserClaim> claims, ICollection<Entity.UserRole> roles) where TUser : IdentityUser
        {
            if (user == null)
            {
                return null;
            }
            return new Entity.User
            {
                Id = user.Id,
                AccessFailedCount = user.AccessFailedCount,
                Email = user.Email,
                UserName = user.UserName,
                TwoFactorEnabled = user.TwoFactorEnabled,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnabled = user.LockoutEnabled,
                ConcurrencyStamp = user.ConcurrencyStamp,
                NormalizedEmail = user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash,
                SecurityStamp = user.SecurityStamp,
                LockoutEnd = user.LockoutEnd.HasValue ? user.LockoutEnd.Value.DateTime : (DateTime?)null,
                UserClaims = claims,
                UserRoles = roles
            };
        }

        public static void AddEntityId<TEntity>(this ICollection<TEntity> collection, string id) where TEntity: Entity.IEntityId, new()
        {
            collection.Add(new TEntity
            {
                Id = id
            });
        }

        public static void RemoveEntityId<TEntity>(this ICollection<TEntity> collection, string id) where TEntity : Entity.IEntityId
        {
            collection.Remove(collection.First(e => e.Id == id));
        }        
    }
}
