// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    public interface IWsFederationService
    {
        Task<IActionResult> ProcessRequest(HttpRequest request, IUrlHelper helper);
    }
}