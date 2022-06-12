using System;

namespace Aguacongas.TheIdServer.BlazorApp.Pages.ExternalProvider.Components
{
    public partial class WindowsOptionsComponent
    {
        string ClaimsCacheAbsoluteExpiration 
        {
            get => Model.Options.ClaimsCacheAbsoluteExpiration.ToString();
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan result))
                {
                    Model.Options.ClaimsCacheAbsoluteExpiration = result;
                }
            }
        }

        string ClaimsCacheSlidingExpiration
        {
            get => Model.Options.ClaimsCacheSlidingExpiration.ToString();
            set
            {
                if (TimeSpan.TryParse(value, out TimeSpan result))
                {
                    Model.Options.ClaimsCacheSlidingExpiration = result;
                }
            }
        }
    }
}
