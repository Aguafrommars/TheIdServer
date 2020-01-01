using System;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class SortEventArgs : EventArgs
    {
        public string OrderBy { get; set; }
    }
}
