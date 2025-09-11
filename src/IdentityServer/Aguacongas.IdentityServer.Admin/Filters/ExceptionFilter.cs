// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Errors.Model;
using System;
using System.Linq;
using System.Net;

namespace Aguacongas.IdentityServer.Admin.Filters
{
    /// <summary>
    /// Exception filter
    /// </summary>
    /// <seealso cref="IExceptionFilter" />
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionFilter"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">logger</exception>
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

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
                ProcessApiException(context, exception);
            }
        }

        private void ProcessApiException(ExceptionContext context, Exception exception)
        {
            _logger.LogError(exception, "{Message}", exception.Message);

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
            if (exception is RegistrationException registrationException)
            {
                context.Result = new BadRequestObjectResult(new RegistrationProblemDetail
                {
                    Error = registrationException.ErrorCode,
                    Error_description = registrationException.Message
                });
                return;
            }

            if (exception is NotFoundException notFoundException)
            {
                context.Result = new NotFoundObjectResult(notFoundException);
            }

            if (exception is ForbiddenException forbiddenException)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Exception = null;
            }
        }
    }
}