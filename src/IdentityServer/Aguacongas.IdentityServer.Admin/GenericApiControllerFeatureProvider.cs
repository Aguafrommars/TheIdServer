using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Generic API contoller feature
    /// </summary>
    /// <seealso cref="IApplicationFeatureProvider{ControllerFeature}" />
    public class GenericApiControllerFeatureProvider<TUser, TRole> :
        IApplicationFeatureProvider<ControllerFeature>
    {
        /// <summary>
        /// Updates the <paramref name="feature" /> instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="T:ApplicationPart" /> instances in the application.</param>
        /// <param name="feature">The feature instance to populate.</param>
        /// <remarks>
        /// <see cref="T:ApplicationPart" /> instances in <paramref name="parts" /> appear in the same ordered sequence they
        /// are stored in <see cref="P:ApplicationParts" />. This ordering may be used by the feature
        /// provider to make precedence decisions.
        /// </remarks>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var entyTypeList = Utils.GetEntityTypeList();
            // This is designed to run after the default ControllerTypeProvider, 
            // so the list of 'real' controllers has already been populated.
            foreach (var entityType in entyTypeList)
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    // There's no controller for this entity, so add the generic version.
                    var controllerType = typeof(GenericApiController<>)
                        .MakeGenericType(entityType.GetTypeInfo())
                        .GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }

            var identityUserControllerType = typeof(IdentityUserController<>)
                    .MakeGenericType(typeof(TUser).GetTypeInfo())
                    .GetTypeInfo();
            feature.Controllers.Add(identityUserControllerType);

            var identityRoleControllerType = typeof(IdentityRoleController<>)
                    .MakeGenericType(typeof(TRole).GetTypeInfo())
                    .GetTypeInfo();
            feature.Controllers.Add(identityRoleControllerType);
        }
    }

}
