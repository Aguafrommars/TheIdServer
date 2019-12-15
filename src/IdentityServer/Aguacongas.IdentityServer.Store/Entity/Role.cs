using System;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Role entity
    /// </summary>
    /// <seealso cref="IEntityId" />
    public class Role : IEntityId, ICloneable<Role>
    {
        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public Role Clone()
        {
            return MemberwiseClone() as Role;
        }
    }
}
