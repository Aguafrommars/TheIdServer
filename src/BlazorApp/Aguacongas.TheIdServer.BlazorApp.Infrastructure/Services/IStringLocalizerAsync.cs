using System;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services
{
    public interface IStringLocalizerAsync
    {
        string this[string name] { get; }
        string this[string name, params object[] arguments] { get; }
    }

    public interface ISharedStringLocalizerAsync: IStringLocalizerAsync
    {
        event Action ResourceReady;

        Task Reset();
    }

    public interface IStringLocalizerAsync<T> : IStringLocalizerAsync
    {
        Action OnResourceReady { get; set; }
    }
}