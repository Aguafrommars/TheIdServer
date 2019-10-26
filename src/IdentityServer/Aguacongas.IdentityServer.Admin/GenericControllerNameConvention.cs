using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Generic API controller name convention
    /// </summary>
    /// <seealso cref="System.Attribute" />
    /// <seealso cref="Microsoft.AspNetCore.Mvc.ApplicationModels.IControllerModelConvention" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericApiControllerNameConvention : Attribute, IControllerModelConvention
    {
        /// <summary>
        /// Called to apply the convention to the <see cref="T:ControllerModel" />.
        /// </summary>
        /// <param name="controller">The <see cref="T:ControllerModel" />.</param>
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() !=
                typeof(GenericApiController<>))
            {
                return;
            }

            var entityType = controller.ControllerType.GenericTypeArguments[0];
            controller.ControllerName = entityType.Name;
        }
    }
}
