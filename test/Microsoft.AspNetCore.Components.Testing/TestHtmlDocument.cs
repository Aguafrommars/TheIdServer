// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using HtmlAgilityPack;

namespace Microsoft.AspNetCore.Components.Testing
{
    internal class TestHtmlDocument : HtmlDocument
    {
        public TestHtmlDocument(TestRenderer renderer)
        {
            Renderer = renderer;
        }

        public TestRenderer Renderer { get; }
    }
}
