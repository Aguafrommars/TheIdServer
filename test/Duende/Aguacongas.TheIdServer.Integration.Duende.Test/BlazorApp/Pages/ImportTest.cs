// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store;
using Aguacongas.TheIdServer.BlazorApp.Pages.Import;
using Bunit;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aguacongas.TheIdServer.IntegrationTest.BlazorApp.Pages
{
    [Collection(BlazorAppCollection.Name)]
    public class ImportTest : TestContext
    {
        public TheIdServerFactory Factory { get; }

        public ImportTest(TheIdServerFactory factory)
        {
            Factory = factory;
        }

        [Fact]
        public async Task HandleFileSelected_should_report_importation_result()
        {
            var component = CreateComponent("Alice Smith",
                SharedConstants.WRITERPOLICY);

            var fileMock = new Mock<IBrowserFile>();
            fileMock.SetupGet(m => m.Name).Returns("test.json");
            fileMock.SetupGet(m => m.ContentType).Returns("application/json");
            fileMock.Setup(m => m.OpenReadStream(It.IsAny<long>(), It.IsAny<CancellationToken>())).Returns(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
            var inputFile = component.FindComponent<InputFile>();
            await inputFile.InvokeAsync(() => inputFile.Instance.OnChange.InvokeAsync(new InputFileChangeEventArgs(new List<IBrowserFile>
            {
                fileMock.Object
            })));

            component.WaitForState(() => component.Markup.Contains("text-success"));
            Assert.Contains("text-success", component.Markup);
        }

        private IRenderedComponent<Import> CreateComponent(string userName,
            string role)
        {
            Factory.ConfigureTestContext(userName,
               new Claim[]
               {
                    new Claim("scope", SharedConstants.ADMINSCOPE),
                    new Claim("role", SharedConstants.READERPOLICY),
                    new Claim("role", role),
                    new Claim("sub", Guid.NewGuid().ToString())
               },
               this);

            var component = RenderComponent<Import>();
            component.WaitForState(() => !component.Markup.Contains("Loading..."), TimeSpan.FromMinutes(1));
            return component;
        }
    }
}
