// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre

using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebClient.Controllers
{
    public class HomeController(IHttpClientFactory httpClientFactory) : Controller
    {
        [AllowAnonymous]
        public IActionResult Index() => View();

        public IActionResult Secure() => View();
        
        public async Task<IActionResult> Renew()
        {
            await HttpContext.GetUserAccessTokenAsync(new UserTokenRequestParameters { ForceTokenRenewal = true });
            return RedirectToAction(nameof(Secure));
        }

        public IActionResult Logout() => SignOut("oidc");

        public async Task<IActionResult> CallApi()
        {
            var client = httpClientFactory.CreateClient("client");

            var response = await client.GetStringAsync("identity");
            ViewBag.Json = response.PrettyPrintJson();
            
            return View();
        }


    }
}