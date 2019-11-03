using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class Notifier
    {
        public Action<Notification> Show { get; set; }

        public void Notify(Notification notification)
        {
            Show?.Invoke(notification);
        }
    }
}
