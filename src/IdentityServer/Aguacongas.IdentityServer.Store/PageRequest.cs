// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Represents a request producing page response
    /// </summary>
    public class PageRequest : GetRequest
    {
        /// <summary>
        /// Gets or sets the number of items to take. Default value is 1000.
        /// </summary>
        public int? Take { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the number of items to skip
        /// </summary>
        public int? Skip { get; set; }

        /// <summary>
        /// Gets or sets the order expression. (Use Pascal case properties names)
        /// </summary>
        public string OrderBy { get; set; }

        /// <summary>
        /// Gets or sets the filter. (Use Pascal case properties names)
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        public string Filter { get; set; }

        /// <summary>
        /// Gets or sets the select. (Use Pascal case properties names)
        /// </summary>
        /// <value>
        /// The select.
        /// </value>
        public string Select { get; set; }
    }
}
