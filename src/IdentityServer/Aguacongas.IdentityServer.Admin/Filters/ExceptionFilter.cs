using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    /// <summary>
    /// Exception filter
    /// </summary>
    /// <seealso cref="IExceptionFilter" />
    public class ExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// Called after an action has thrown an <see cref="T:System.Exception" />.
        /// </summary>
        /// <param name="context">The <see cref="T:Microsoft.AspNetCore.Mvc.Filters.ExceptionContext" />.</param>
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor &&
                actionDescriptor.ControllerTypeInfo
                    .FullName.StartsWith("Aguacongas.IdentityServer.Admin"))
            {
                if (exception is InvalidOperationException)
                {
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails
                    {
                        Detail = exception.Message
                    });
                    return;
                }
                if (exception is IdentityException identityException)
                {
                    if (identityException.Errors != null)
                    {
                        context.Result = new BadRequestObjectResult(new ValidationProblemDetails
                        (
                            identityException.Errors.ToDictionary(e => e.Code, e => new string[] { e.Description })
                        ));
                        return;
                    }
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails
                    {
                        Detail = exception.Message
                    });
                    return;
                }
                if (exception is DbUpdateException dbUpdateException)
                {
                    if (dbUpdateException.InnerException == null)
                    {
                        context.Result = new ConflictObjectResult(new ProblemDetails
                        {
                            Detail = dbUpdateException.Message
                        });
                        return;
                    }
                    context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState)
                    {
                        Detail = dbUpdateException.InnerException.Message ?? dbUpdateException.Message
                    });
                }
            }
        }
    }
}
