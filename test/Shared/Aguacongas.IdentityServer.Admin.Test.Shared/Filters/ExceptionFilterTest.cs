// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Aguacongas.IdentityServer.Admin.Test.Filters
{
    public class ExceptionFilterTest
    {
        public static IEnumerable<object[]> GetBadResultException()
        {
            yield return new object[] { new InvalidOperationException("test") };
            yield return new object[] { new IdentityException("test") };
            yield return new object[] { new IdentityException("test")
                {
                    Errors = new IdentityError[] { new IdentityError
                        {
                            Code = "test",
                            Description = "test"
                        }
                    }
                }
            };
            yield return new object[] { new DbUpdateException("test", new Exception()) };
        }

        [Theory]
        [MemberData(nameof(GetBadResultException))]
        public void OnException_should_set_context_bad_request_result_according_to_exception_type(Exception e)
        {
            var errorContext = CreateExceptionContext(e);
            var loggerMock = new Mock<ILogger<ExceptionFilter>>();
            var sut = new ExceptionFilter(loggerMock.Object);
            sut.OnException(errorContext);

            Assert.IsType<BadRequestObjectResult>(errorContext.Result);
        }

        [Fact]
        public void OnException_should_set_context_conflict_request_result_according_to_exception_type()
        {
            var errorContext = CreateExceptionContext(new DbUpdateException());
            var loggerMock = new Mock<ILogger<ExceptionFilter>>();
            var sut = new ExceptionFilter(loggerMock.Object);
            sut.OnException(errorContext);

            Assert.IsType<ConflictObjectResult>(errorContext.Result);
        }

        [Fact]
        public void OnException_should_not_set_context_result_for_unkown_controller()
        {
            var errorContext = CreateExceptionContext(new DbUpdateException());
            ((ControllerActionDescriptor)errorContext.ActionDescriptor).ControllerTypeInfo = typeof(object).GetTypeInfo();
            var loggerMock = new Mock<ILogger<ExceptionFilter>>();
            var sut = new ExceptionFilter(loggerMock.Object);
            sut.OnException(errorContext);

            Assert.Null(errorContext.Result);

            errorContext.ActionDescriptor = new ActionDescriptor();

            sut.OnException(errorContext);

            Assert.Null(errorContext.Result);
        }

        private static ExceptionContext CreateExceptionContext(Exception e)
        {
            var httpContextMock = new Mock<HttpContext>();
            var errorContext = new ExceptionContext(new ActionContext
            {
                HttpContext = httpContextMock.Object,
                RouteData = new Microsoft.AspNetCore.Routing.RouteData(),
                ActionDescriptor = new ActionDescriptor()
            }, new List<IFilterMetadata>())
            {
                ActionDescriptor = new ControllerActionDescriptor
                {
                    ControllerTypeInfo = typeof(ExternalProviderKindController).GetTypeInfo()
                },
                Exception = e,

            };
            return errorContext;
        }
    }
}
