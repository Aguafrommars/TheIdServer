// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System.Collections.Generic;

namespace Aguacongas.IdentityServer.Store.Entity
{
    /// <summary>
    /// Localizable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILocalizable<T> where T: IEntityResource
    {
        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        ICollection<T> Resources { get; }
    }
}
