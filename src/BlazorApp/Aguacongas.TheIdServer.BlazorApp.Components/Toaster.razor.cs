using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.BlazorApp.Components
{
    public partial class Toaster
    {
        private readonly List<Notification> _notifications = new List<Notification>();

        protected override void OnInitialized()
        {
            _notifier.Show = async notification =>
            {
                _notifications.Add(notification);
                await InvokeAsync(StateHasChanged).ConfigureAwait(false);
            };
        }

        private void OnToastClosed(Notification notification)
        {
            _notifications.Remove(notification);
            InvokeAsync(StateHasChanged);
        }
    }
}
