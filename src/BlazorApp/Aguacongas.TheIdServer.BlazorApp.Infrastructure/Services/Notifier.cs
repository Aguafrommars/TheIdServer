using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class Notifier
    {
        public Func<Notification, Task> Show { get; set; }

        public Task NotifyAsync(Notification notification)
        {
            return Show?.Invoke(notification);
        }
    }
}
