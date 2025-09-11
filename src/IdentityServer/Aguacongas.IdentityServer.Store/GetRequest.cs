// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Define a get request.
    /// </summary>
    public class GetRequest
    {
        /// <summary>
        /// Gets or sets the expand. (Use Pascal case properties names)
        /// </summary>
        /// <value>
        /// The expand.
        /// </value>
        public string Expand { get; set; }

        /// <summary>
        /// Gets or sets the format.
        /// </summary>
        /// <value>
        /// The format. (json | export)
        /// </value>
        public string Format { get; set; }
    }
}
