// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.KeysRotation;
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
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
    public class GenericControllerFeatureProvider :
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
            var entyTypeList = Utils.GetEntityTypeList()
                .Where(e => e != typeof(User) || e!= typeof(Role));
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

            var keyTypeList = new[]
            {
                typeof(IAuthenticatedEncryptorDescriptor),
                typeof(RsaEncryptorDescriptor)
            };
            // This is designed to run after the default ControllerTypeProvider, 
            // so the list of 'real' controllers has already been populated.
            foreach (var keyType in keyTypeList)
            {
                var typeName = keyType.Name + "Controller";
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    // There's no controller for this key, so add the generic version.
                    var controllerType = typeof(GenericKeyController<>)
                        .MakeGenericType(keyType.GetTypeInfo())
                        .GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }
    }

}
