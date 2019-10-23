using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Models
{
    public class SortEventArgs : EventArgs
    {
        public string OrderBy { get; set; }
    }
}
