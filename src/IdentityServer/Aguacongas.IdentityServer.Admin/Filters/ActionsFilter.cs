using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    class ActionsFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controlerType = context.Controller.GetType();
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (context.Exception == null &&
                !context.Canceled &&
                actionDescriptor.ActionName == "Create" &&                
                controlerType.FullName
                .StartsWith("Aguacongas.IdentityServer.Admin.GenericApiController",
                    StringComparison.Ordinal))
            {
                var objectResult = context.Result as ObjectResult;
                var value = objectResult.Value as IEntityId;
                var request = context.HttpContext.Request;
                context.Result = new CreatedResult($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}/{value.Id}", objectResult.Value);
            }
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controlerType = context.Controller.GetType();
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.ActionName == "Update" &&
                controlerType.FullName
                .StartsWith("Aguacongas.IdentityServer.Admin.GenericApiController",
                    StringComparison.Ordinal) &&
                context.ActionArguments["entity"] is IEntityId entity &&
                entity.Id != context.ActionArguments["id"] as string)
            {
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails
                {
                    Detail = "Ids don't match"
                });
            }
        }
    }
}
