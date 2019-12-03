using Aguacongas.IdentityServer.Admin;
using Microsoft.AspNetCore.Routing;

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
            foreach(var entityType in entyTypeList)
            {
                endpoints.MapControllerRoute(entityType.Name, $"{entityType.Name}");
            }
            endpoints.MapControllerRoute("IdentityProvider", "IdentityProvider");
            return endpoints;
        }        
    }
}
