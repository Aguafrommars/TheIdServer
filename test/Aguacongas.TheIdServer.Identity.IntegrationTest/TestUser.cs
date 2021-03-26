// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TestUser : IdentityUser
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public override bool Equals(object obj)
        {
            if (obj is IdentityUser<string> other)
            {
                return other.Email == (string.IsNullOrEmpty(Email) ? null : Email)
                    && other.Id == Id
                    && other.PasswordHash == PasswordHash
                    && other.PhoneNumber == (string.IsNullOrEmpty(PhoneNumber) ? null : PhoneNumber)
                    && other.UserName == UserName;
            }
            return false;
        }
    }
}
