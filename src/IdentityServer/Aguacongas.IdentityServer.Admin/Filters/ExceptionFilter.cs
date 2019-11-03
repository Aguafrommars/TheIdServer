using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                actionDescriptor.ControllerTypeInfo
                    .FullName.StartsWith("Aguacongas.IdentityServer.Admin.GenericApiController"))
            {
                if (exception is InvalidOperationException)
                {
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails
                    {
                        Detail = exception.Message
                    });
                }
                if (exception is DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException == null)
                    {
                        context.Result = new ConflictObjectResult(new ProblemDetails
                        {
                            Detail = dbUpdateException.Message
                        });
                    }
                    else
                    {
                        context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState)
                        {
                            Detail = dbUpdateException.InnerException.Message ?? dbUpdateException.Message
                        });
                    }
                }
            }
        }
    }
}
