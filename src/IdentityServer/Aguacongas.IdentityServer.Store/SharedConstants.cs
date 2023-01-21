// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
namespace Aguacongas.IdentityServer.Store
{
    /// <summary>
    /// Contains shared constants.
    /// </summary>
    public static class SharedConstants
    {
        /// <summary>
        /// The writer
        /// </summary>
        public const string WRITERPOLICY = "Is4-Writer";
        /// <summary>
        /// The reader
        /// </summary>
        public const string READERPOLICY = "Is4-Reader";

        /// <summary>
        /// The registration
        /// </summary>
        public const string REGISTRATIONPOLICY = "Is4-Registration";

        /// <summary>
        /// The registration
        /// </summary>
        public const string TOKENPOLICY = "Is4-Token";

        /// <summary>
        /// The admon scope
        /// </summary>
        public const string ADMINSCOPE = "theidserveradminapi";

        /// <summary>
        /// The token scope
        /// </summary>
        public const string TOKENSCOPES = "theidservertokenapi";

        /// <summary>
        /// Defines the dynamic configuration reader authorization policy
        /// </summary>
        public const string DYNAMIC_CONFIGURATION_READER_POLICY = "DynamicConfigurationReaderPolicy";
        /// <summary>
        /// Defines the dynamic configuration writter authorization policy
        /// </summary>
        public const string DYNAMIC_CONFIGURATION_WRITTER_POLICY = "DynamicConfigurationWritterPolicy";
    }
}
