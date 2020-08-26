// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Represents a page of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The type of data</typeparam>
    public class PageResponse<T>
    {
        /// <summary>
        /// Gets or sets page items
        /// </summary>
        public IEnumerable<T> Items { get; set; }
        /// <summary>
        /// Gets or sets total number of items
        /// </summary>
        public int Count { get; set; }
    }
}
