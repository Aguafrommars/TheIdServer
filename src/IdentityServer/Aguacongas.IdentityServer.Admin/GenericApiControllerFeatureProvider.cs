using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Generic API contoller feature
    /// </summary>
    /// <seealso cref="IApplicationFeatureProvider{ControllerFeature}" />
    public class GenericApiControllerFeatureProvider :
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
            var entyTypeList = GetEntityTypeList();
            // This is designed to run after the default ControllerTypeProvider, 
            // so the list of 'real' controllers has already been populated.
            foreach (var entityType in entyTypeList)
            {
                var typeName = entityType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    // There's no controller for this entity, so add the generic version.
                    var controllerType = typeof(GenericApiController<>)
                        .MakeGenericType(entityType.GetTypeInfo()).GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }

        /// <summary>
        /// Gets the entity type list.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> GetEntityTypeList()
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entyTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract &&
                t.GetInterface("IEntityId") != null);
            return entyTypeList;
        }
    }

}
