using System.Collections;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class GrantTypes : IReadOnlyDictionary<string, string>
    {
        private static Dictionary<string, string> _grantTypes = new Dictionary<string, string>
        {
            ["authorization_code"] = "Code",
            ["client_credentials"] = "Client credentials",
            ["hybrid"] = "Hybrid",
            ["implicit"] = "Implicit",
            ["password"] = "Resource owner password",
            ["urn:ietf:params:oauth:grant-type:device_code"] = "Device flow"
        };

        public string this[string key] => _grantTypes[key];

        public static GrantTypes Instance { get; } = new GrantTypes();

        public IEnumerable<string> Keys => _grantTypes.Keys;

        public IEnumerable<string> Values => _grantTypes.Values;

        public int Count => _grantTypes.Count;

        public bool ContainsKey(string key)
            => _grantTypes.ContainsKey(key);

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => _grantTypes.GetEnumerator();

        public bool TryGetValue(string key, out string value)
            => _grantTypes.TryGetValue(key, out value);

        public static string GetGrantTypeName(string key)
        {
            var grantTypes = GrantTypes.Instance;

            if (grantTypes.ContainsKey(key))
            {
                return grantTypes[key];
            }
            return key;
        }
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
