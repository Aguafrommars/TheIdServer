using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.IdentityServer.Admin
{
    public class GenericApiControllerFeatureProvider :
        IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var assembly = typeof(IEntityId).GetTypeInfo().Assembly;
            var entyTypeList = assembly.GetTypes().Where(t => t.IsClass &&
                !t.IsAbstract && 
                t.GetInterface("IEntityId") != null);
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
    }

}
