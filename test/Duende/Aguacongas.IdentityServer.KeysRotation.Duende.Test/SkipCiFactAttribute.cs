using System;
using Xunit;

namespace Aguacongas.IdentityServer.KeysRotation.Duende.Test
{
    public class SkipCiFactAttribute : FactAttribute
    {
        public SkipCiFactAttribute()
        {
            var ci = Environment.GetEnvironmentVariable("CI");
            if (!string.IsNullOrEmpty(ci))
            {
                Skip = $"Skipped on {ci}";
            }
        }
    }
}
