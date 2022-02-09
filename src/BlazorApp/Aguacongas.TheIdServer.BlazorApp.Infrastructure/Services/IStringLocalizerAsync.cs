// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.TheIdServer.BlazorApp.Infrastructure.Services
{
    public interface ISharedStringLocalizerAsync: IStringLocalizer
    {
        event Action ResourceReady;

        Task Reset();
        Task<IEnumerable<string>> GetSupportedCulturesAsync();
    }

    public interface IStringLocalizerAsync<T> : IStringLocalizer
    {
        Action OnResourceReady { get; set; }
    }
}