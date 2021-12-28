namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Options for how persisted grants are persisted.
    /// </summary>
    public class PersistentGrantOptions
    {
        /// <summary>
        /// Data protect the persisted grants "data" column.
        /// </summary>
        public bool DataProtectData { get; set; }
    }
}