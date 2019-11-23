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

            if (!(context.Result is ObjectResult objectResult) || 
                context.Exception != null ||
                context.Canceled ||
                !controlerType.FullName
                    .StartsWith("Aguacongas.IdentityServer.Admin",
                    StringComparison.Ordinal))
            {
                return;
            }

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.ActionName == "Create")
            {
                var value = objectResult.Value as IEntityId;
                var request = context.HttpContext.Request;
                context.Result = new CreatedResult($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}/{value.Id}", objectResult.Value);
                return;
            }

            if (actionDescriptor.ActionName == "Get" && objectResult.Value == null)
            {
                context.Result = new NotFoundObjectResult(new ProblemDetails 
                {
                    Title = "Entity not found",
                    Status = 404
                });
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
