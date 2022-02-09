// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
namespace Aguacongas.TheIdServer.UI
{
    public static class ConsentOptions
    {
        public static bool EnableOfflineAccess { get; set; } = true;
        public static string OfflineAccessDisplayName { get; set; } = "Offline Access";
        public static string OfflineAccessDescription { get; set; } = "Access to your applications and resources, even when you are offline";

        public static string MustChooseOneErrorMessage { get; } = "You must pick at least one permission";

        public static string InvalidSelectionErrorMessage { get; } = "Invalid selection";
    }
}
