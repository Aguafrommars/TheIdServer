// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Identity;

namespace Aguacongas.TheIdServer.Identity.IntegrationTest
{
#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public class TestRole: IdentityRole
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        public TestRole():base()
        { }

        public TestRole(string name):base(name)
        { }

        public override bool Equals(object obj)
        {
            if (obj is IdentityRole<string> other)
            {
                return other.Id == Id
                    && other.Name == Name;
            }

            return false;
        }
    }
}
