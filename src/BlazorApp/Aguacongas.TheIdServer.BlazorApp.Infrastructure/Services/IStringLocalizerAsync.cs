using System;

namespace Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services
{
    public interface IStringLocalizerAsync
    {
        string this[string name] { get; }
        string this[string name, params object[] arguments] { get; }
    }

    public interface ISharedStringLocalizerAsync
    {
        event Action ResourceReady;
    }

    public interface IStringLocalizerAsync<T> : IStringLocalizerAsync
    {
        Action OnResourceReady { get; set; }
    }
}