using System;
using System.Collections.Generic;
using System.Text;

namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Cloneable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
