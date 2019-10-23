using Aguacongas.TheIdServer.BlazorApp.Models;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Services
{
    public interface IManageSettings
    {
        Settings Settings { get;  }

        Task InitializeAsync();
    }
}