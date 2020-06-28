using System.Security.Cryptography.X509Certificates;

namespace Aguacongas.IdentityServer.Admin.Configuration
{
    internal class KeyDefinition
    {
        public KeyKinds? Type { get; set; }
        public bool? Persisted { get; set; }
        public string FilePath { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public StoreLocation? StoreLocation { get; set; }
        public string StoreName { get; set; }
        public string StorageFlags { get; set; }
    }
}
