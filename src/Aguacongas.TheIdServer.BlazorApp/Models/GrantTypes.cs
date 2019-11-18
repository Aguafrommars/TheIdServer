using System.Collections;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class GrantTypes : IReadOnlyList<string>
    {
        private static List<string> _grantTypes = new List<string>
        {
            "authorization_code",
            "client_credentials",
            "hybrid",
            "implicit",
            "password",
            "urn:ietf:params:oauth:grant-type:device_code"
        };

        public static GrantTypes Instance { get; } = new GrantTypes();
        public string this[int index] => _grantTypes[index];

        public int Count => _grantTypes.Count;

        public IEnumerator<string> GetEnumerator() 
            => _grantTypes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
