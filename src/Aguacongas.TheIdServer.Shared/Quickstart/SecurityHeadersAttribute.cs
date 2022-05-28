// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
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
                    context.HttpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Frame-Options
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Frame-Options"))
                {
                    context.HttpContext.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy
                var builder = new StringBuilder("default-src 'self'");
#if DEBUG
                builder.Append(" wss://localhost:44337/Aguacongas.TheIdServer.Duende/");
#endif
                builder.Append("; object-src 'none'; frame-ancestors 'none'; sandbox allow-forms allow-same-origin allow-scripts; base-uri 'self';upgrade-insecure-requests;");
                builder.Append("style-src 'self' https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/css/bootstrap.min.css;");
                var autorizeScriptsUrl = new[]
                {
                    "'sha256-vwa3kDBkD7mP1Y0njpcyAH7GXn3/HkE72HGlVShVMUg='",
                    "https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js",
                    "https://cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.slim.min.js",
                };
                builder.Append("script-src 'self'");
                foreach(var url in autorizeScriptsUrl)
                {
                    builder.Append(' ');
                    builder.Append(url);
                }
                // also an example if you need client images to be displayed from twitter
                // builder.Append(";img-src 'self' https://pbs.twimg.com;");
                var csp = builder.ToString();
                
                // once for standards compliant browsers
                if (!context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Content-Security-Policy", csp);
                }
                // and once again for IE
                if (!context.HttpContext.Response.Headers.ContainsKey("X-Content-Security-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("X-Content-Security-Policy", csp);
                }

                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Referrer-Policy
                var referrer_policy = "no-referrer";
                if (!context.HttpContext.Response.Headers.ContainsKey("Referrer-Policy"))
                {
                    context.HttpContext.Response.Headers.Add("Referrer-Policy", referrer_policy);
                }
            }
        }
    }
}
