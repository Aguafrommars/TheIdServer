﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.IdentityServer.Store.Entity
{
    public static class EntityExtensions
    {
        public static Role ToRole<TRole>(this TRole role) where TRole : IdentityRole
        {
            return new Role
            {
                ConcurrencyStamp = role.ConcurrencyStamp,
                Id = role.Id,
                Name = role.Name,
                NormalizedName = role.NormalizedName
            };
        }
        public static TRole ToIdentityRole<TRole>(this Role entity) where TRole: IdentityRole, new()
        {
            return new TRole
            {
                ConcurrencyStamp = entity.ConcurrencyStamp,
                Id = entity.Id,
                Name = entity.Name,
                NormalizedName = entity.NormalizedName
            };
        }

        public static User ToUser<TUser>(this TUser user) where TUser: IdentityUser
        {
            return new User
            {
                AccessFailedCount = user.AccessFailedCount,
                ConcurrencyStamp = user.ConcurrencyStamp,
                Email = string.IsNullOrEmpty(user.Email) ? null : user.Email,
                EmailConfirmed = user.EmailConfirmed,
                Id = user.Id,
                LockoutEnabled = user.LockoutEnabled,
                LockoutEnd = user.LockoutEnd.HasValue ? user.LockoutEnd.Value.DateTime : null,
                NormalizedEmail = string.IsNullOrEmpty(user.NormalizedEmail) ? null : user.NormalizedEmail,
                NormalizedUserName = user.NormalizedUserName,
                PasswordHash = user.PasswordHash,
                PhoneNumber = string.IsNullOrEmpty(user.PhoneNumber) ? null : user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                SecurityStamp = user.SecurityStamp,
                TwoFactorEnabled = user.TwoFactorEnabled,
                UserName = user.UserName
            };           
        }

        public static UserToken ToEntity(this IdentityUserToken<string> token)
        {
            return new UserToken
            {
                Id = $"{token.UserId}@{token.LoginProvider}@{token.Name}",
                LoginProvider = token.LoginProvider,
                Name = token.Name,
                UserId = token.UserId,
                Value = token.Value
            };
        }
    }
}
