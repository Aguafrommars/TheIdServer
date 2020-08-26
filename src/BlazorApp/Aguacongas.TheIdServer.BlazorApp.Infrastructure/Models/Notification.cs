// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class Notification
    {
        public Guid Id { get; } = Guid.NewGuid();

        public bool IsError { get; set; }

        public string Header { get; set; }
        
        public string Message { get; set; }
    }
}
