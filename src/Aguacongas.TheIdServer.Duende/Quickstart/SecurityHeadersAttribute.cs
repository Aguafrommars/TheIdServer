// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;

namespace Aguacongas.TheIdServer.UI
{
    public class SecurityHeadersAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            var result = context.Result;
            if (result is ViewResult)
            {
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Content-Type-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Type-Options"))
                {
                    context.HttpContext.Response.Headers["X-Content-Type-Options"] = "nosniff";
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers["X-Frame-Options"] = "SAMEORIGIN";
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var builder = new StringBuilder("default-src 'self'");
                builder.Append("; object-src 'none'; frame-ancestors 'self'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';upgrade-insecure-requests;");
                builder.Append("img-src 'self' data:; style-src 'self' ");
                builder.Append(SiteOptions.BOOTSTRAPCSSURL);
                builder.Append(';');

                var autorizeScriptsUrl = new[]
                {
                    "'sha256-vwa3kDBkD7mP1Y0njpcyAH7GXn3/HkE72HGlVShVMUg='",
                    SiteOptions.BOOTSTRAPJSURL,
                    SiteOptions.JQUERYURL,
                };
                builder.Append("script-src 'self'");
                foreach(var url in autorizeScriptsUrl)
                {
                    builder.Append(' ');
                    builder.Append(url);
                }
#if DEBUG
                builder.Append("; connect-src *");
#endif
                var csp = builder.ToString();
                
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers["Content-Security-Policy"] = csp;
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers["X-Content-Security-Policy"] = csp;
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers["Referrer-Policy"] = referrer_policy;
                }
            }
        }
    }
}
