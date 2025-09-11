// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;

namespace Aguacongas.TheIdServer.WsFederationSample.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            var props = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            return Challenge(props, WsFederationDefaults.AuthenticationScheme);
        }

        public IActionResult LogOut()
        {
            return SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme,
                WsFederationDefaults.AuthenticationScheme);
        }

        public IActionResult SignOutCleanup(string sid, string iss)
        {
            return Redirect("/");
        }
    }
}