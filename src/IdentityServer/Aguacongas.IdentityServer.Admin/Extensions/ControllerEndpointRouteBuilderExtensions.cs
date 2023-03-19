// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// Contains extension methods for using Controllers with <see cref="IEndpointRouteBuilder"/>.
    /// </summary>
    public static class ControllerEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// Maps the admin API controllers.
        /// </summary>
        /// <param name="endpoints">The endpoints.</param>
        /// <returns></returns>
        public static IEndpointRouteBuilder MapAdminApiControllers(this IEndpointRouteBuilder endpoints)
        {
            var entyTypeList = Utils.GetEntityTypeList();
            foreach(var name in entyTypeList.Select(e => e.Name))
            {
                endpoints.MapControllerRoute(name, $"{name}");
            }
            endpoints.MapControllerRoute("IdentityProvider", "IdentityProvider");
            return endpoints;
        }        
    }
}
