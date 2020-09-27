using System.Security.Cryptography.Xml;

namespace Aguacongas.IdentityServer.KeysRotation
{
    public interface IInternalEncryptedXmlDecryptor
    {
        void PerformPreDecryptionSetup(EncryptedXml encryptedXml);
    }
}