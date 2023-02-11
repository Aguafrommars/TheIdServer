// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="CookieRequestCultureProvider" />
    public class SetCookieFromQueryStringRequestCultureProvider : CookieRequestCultureProvider
    {
        /// <summary>
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        /// <inheritdoc />
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            if (httpContext.Request.Query.TryGetValue("culture", out StringValues culture))
            {
                httpContext.Response.Cookies.Append(CookieName, MakeCookieValue(new RequestCulture(culture)));
            }
            return base.DetermineProviderCultureResult(httpContext);
        }
    }
}
