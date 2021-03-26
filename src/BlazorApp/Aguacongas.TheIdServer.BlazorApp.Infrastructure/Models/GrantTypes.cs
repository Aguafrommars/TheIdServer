// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using System.Collections;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IReadOnlyDictionary{String, String}" />
    public class GrantTypes : IReadOnlyDictionary<string, string>
    {
        private static readonly Dictionary<string, string> _grantTypes = new Dictionary<string, string>
        {
            ["authorization_code"] = "Code",
            ["client_credentials"] = "Client credentials",
            ["hybrid"] = "Hybrid",
            ["implicit"] = "Implicit",
            ["password"] = "Resource owner password",
            ["urn:ietf:params:oauth:grant-type:device_code"] = "Device flow"
        };

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified key.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string this[string key] => _grantTypes[key];

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static GrantTypes Instance { get; } = new GrantTypes();

        /// <summary>
        /// Gets an enumerable collection that contains the keys in the read-only dictionary.
        /// </summary>
        public IEnumerable<string> Keys => _grantTypes.Keys;

        /// <summary>
        /// Gets an enumerable collection that contains the values in the read-only dictionary.
        /// </summary>
        public IEnumerable<string> Values => _grantTypes.Values;

        /// <summary>
        /// Gets the number of elements in the collection.
        /// </summary>
        public int Count => _grantTypes.Count;

        /// <summary>
        /// Determines whether the read-only dictionary contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>
        /// true if the read-only dictionary contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
            => _grantTypes.ContainsKey(key);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            => _grantTypes.GetEnumerator();

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the object that implements the <see cref="T:System.Collections.Generic.IReadOnlyDictionary`2"></see> interface contains an element that has the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out string value)
            => _grantTypes.TryGetValue(key, out value);

        /// <summary>
        /// Gets the name of the grant type.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
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
