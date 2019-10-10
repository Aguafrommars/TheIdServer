namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Represents a request producing page response
    /// </summary>
    public class PageRequest
    {
        /// <summary>
        /// Gets or sets the number of items to take
        /// </summary>
        public int? Take { get; set; }

        /// <summary>
        /// Gets or sets the number of items to skip
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the order expression
        /// </summary>
        public string OrderExpression { get; set; }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }
    }
}
