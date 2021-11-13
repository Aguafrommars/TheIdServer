// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public class Notifier
    {
        public Func<Notification, Task> Show { get; set; }

        public async Task NotifyAsync(Notification notification)
        {
            if (Show != null)
            {
                await Show.Invoke(notification).ConfigureAwait(false);
            }
        }
    }
}
