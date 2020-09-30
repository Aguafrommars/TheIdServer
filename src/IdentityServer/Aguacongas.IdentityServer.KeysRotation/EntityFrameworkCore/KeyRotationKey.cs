
namespace Aguacongas.IdentityServer.KeysRotation.EntityFrameworkCore
{
    /// <summary>
    /// Code first model used by <see cref="EntityFrameworkCoreXmlRepository{TContext}"/>.
    /// </summary>
    public class KeyRotationKey
    {
        /// <summary>
        /// The entity identifier of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// The XML representation of the <see cref="KeyRotationKey"/>.
        /// </summary>
        public string Xml { get; set; }
    }
}
