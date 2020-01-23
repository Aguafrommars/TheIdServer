using HtmlAgilityPack;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Testing;
using System.Collections.Generic;

namespace Aguacongas.TheIdServer.IntegrationTest
{
    public static class TestHostExtensions
    {
        public static HtmlNode WaitForNode<TComponent>(this TestHost host, RenderedComponent<TComponent> component, string selector) 
            where TComponent : IComponent
        {
            var node = component.Find(selector);
            while (node == null)
            {
                host.WaitForNextRender();
                node = component.Find(selector);
            }

            return node;
        }

        public static ICollection<HtmlNode> WaitForAllNodes<TComponent>(this TestHost host, RenderedComponent<TComponent> component, string selector)
            where TComponent : IComponent
        {
            var nodes = component.FindAll(selector);
            while (nodes.Count == 0)
            {
                host.WaitForNextRender();
                nodes = component.FindAll(selector);
            }

            return nodes;
        }


        public static string WaitForContains<TComponent>(this TestHost host, RenderedComponent<TComponent> component, string term)
            where TComponent : IComponent
        {
            var markup = component.GetMarkup();
            while (!markup.Contains(term))
            {
                host.WaitForNextRender();
                markup = component.GetMarkup();
            }

            return markup;
        }
    }
}
