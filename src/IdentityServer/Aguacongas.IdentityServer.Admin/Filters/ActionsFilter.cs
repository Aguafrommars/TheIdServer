// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionsFilter : IActionFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var controlerType = context.Controller.GetType();
            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (context.Exception != null ||
                context.Canceled ||
                !controlerType.FullName
                    .StartsWith("Aguacongas.IdentityServer.Admin",
                    StringComparison.Ordinal))
            {
                return;
            }
            
            if (actionDescriptor.ActionName == "Delete")
            {
                context.Result = new NoContentResult();
                return;
            }

            if (context.Result is not ObjectResult objectResult)
            {
                return;
            }

            if (actionDescriptor.ActionName == "Create")
            {
                string id = null;
                var value = objectResult.Value;
                if (value != null)
                {
                    var idProperty = value.GetType().GetProperty("Id");
                    id = idProperty?.GetValue(value)?.ToString();
                }
                var request = context.HttpContext.Request;
                context.Result = new CreatedResult($"{request.Scheme}://{request.Host}{request.PathBase}{request.Path}/{id}", value);
                return;
            }

            if (actionDescriptor.ActionName == "Get" && objectResult.Value == null)
            {
                context.Result = new NotFoundObjectResult(new ProblemDetails 
                {
                    Title = "Entity not found",
                    Status = (int)HttpStatusCode.NotFound
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var controlerType = context.Controller.GetType();
            if (!controlerType.FullName
                    .StartsWith("Aguacongas.IdentityServer.Admin",
                    StringComparison.Ordinal))
            {
                // not my business
                return;
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
                return;
            }

            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor.ActionName == "Update" &&
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
