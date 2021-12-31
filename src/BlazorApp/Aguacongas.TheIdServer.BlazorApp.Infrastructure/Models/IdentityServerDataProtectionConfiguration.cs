namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Signing key definition kinds
    /// </summary>
    public enum KeyKinds
    {
        /// <summary>
        /// From X509 file
        /// </summary>
        File,
        /// <summary>
        /// Temp key for development
        /// </summary>
        Development,
        /// <summary>
        /// From X509 store
        /// </summary>
        Store,
        /// <summary>
        /// From keys rotation
        /// </summary>
        KeysRotation
    }

    public class IdentityServerDataProtectionConfiguration : DataProtectionConfigurationBase
    {

        public KeyKinds Type { get; set; }

        public RsaEncryptorConfiguration RsaEncryptorConfiguration { get; set; }
    }
}