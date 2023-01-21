// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Generic API controller name convention
    /// </summary>
    /// <seealso cref="Attribute" />
    /// <seealso cref="IControllerModelConvention" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
    {
        /// <summary>
        /// Called to apply the convention to the <see cref="T:ControllerModel" />.
        /// </summary>
        /// <param name="controller">The <see cref="T:ControllerModel" />.</param>
        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType)
            {             
                return;
            }            

            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericApiController<>) || 
                controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericKeyController<>))
            {
                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
            }
        }
    }
}
