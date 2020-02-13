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
        /// Gets or sets the concurrency stamp.
        /// </summary>
        /// <value>
        /// The concurrency stamp.
        /// </value>
        public string ConcurrencyStamp { get; set; }

        /// <summary>
        /// Gets or sets the name of the normalized.
        /// </summary>
        /// <value>
        /// The name of the normalized.
        /// </value>
        public string NormalizedName { get; set; }

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
