namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Cloneable interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICloneable<out T>
    {
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}
