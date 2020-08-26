// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Api sub entity interface
    /// </summary>
    public interface IApiSubEntity
    {
        /// <summary>
        /// Gets or sets the API identifier.
        /// </summary>
        /// <value>
        /// The API identifier.
        /// </value>
        string ApiId { get; set; }
    }
}
