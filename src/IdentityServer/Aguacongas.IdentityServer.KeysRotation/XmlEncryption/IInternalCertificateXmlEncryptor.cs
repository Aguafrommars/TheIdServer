using System.Security.Cryptography.Xml;
using System.Xml;

namespace Aguacongas.IdentityServer.KeysRotation
{
    internal interface IInternalCertificateXmlEncryptor
    {
        EncryptedData PerformEncryption(EncryptedXml encryptedXml, XmlElement elementToEncrypt);
    }
}
