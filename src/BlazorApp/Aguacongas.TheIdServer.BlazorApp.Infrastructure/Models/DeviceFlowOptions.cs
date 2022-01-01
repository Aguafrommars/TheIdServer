namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    /// <summary>
    /// Configures device flow
    /// </summary>
    public class DeviceFlowOptions
    {
        /// <summary>
        /// Gets or sets the default type of the user code.
        /// </summary>
        /// <value>
        /// The default type of the user code.
        /// </value>
        public string DefaultUserCodeType { get; set; } = "Numeric";

        /// <summary>
        /// Gets or sets the polling interval in seconds.
        /// </summary>
        /// <value>
        /// The interval in seconds.
        /// </value>
        public int Interval { get; set; }
    }
}