using Microsoft.IdentityModel.Protocols.WsFederation;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Aguacongas.IdentityServer.WsFederation.Metadata
{
    /// <summary>
    /// Serializes WS-Federation metadata.
    /// </summary>
    public class MetadataSerializer : IMetatdataSerializer
    {
        /// <inheritdoc/>
        public Task<string> SerializeAsync(WsFederationConfiguration configuration)
        {
            var ser = new WsFederationMetadataSerializer();
            using var ms = new MemoryStream();
            using XmlWriter writer = new MetadataExtensionsWriter(XmlDictionaryWriter.CreateTextWriter(ms, Encoding.UTF8, false), configuration);
            ser.WriteMetadata(writer, configuration);
            writer.Flush();
            return Task.FromResult(Encoding.UTF8.GetString(ms.ToArray()));
        }
    }
}
