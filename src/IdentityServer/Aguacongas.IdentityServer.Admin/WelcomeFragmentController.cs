// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Admin.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using IO = System.IO;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Welcome fragment controller
    /// </summary>
    /// <seealso cref="Controller" />
    [ApiRoute("[controller]")]
    public class WelcomeFragmentController : Controller
    {
        private readonly IWebHostEnvironment _environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomeFragmentController"/> class.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <exception cref="ArgumentNullException">environment</exception>
        public WelcomeFragmentController(IWebHostEnvironment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        /// <summary>
        /// Gets welcome fragment html code depending the current culture and environmeent.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public FileResult Get()
        {
            var path = $"{_environment.WebRootPath}/{_environment.EnvironmentName}-welcome-fragment.{CultureInfo.CurrentCulture.Name}.html";
            if (IO.File.Exists(path))
            {
                return File(IO.File.OpenRead(path), "text/htnl");
            }
            path = $"{_environment.WebRootPath}/{_environment.EnvironmentName}-welcome-fragment.html";
            if (IO.File.Exists(path))
            {
                return File(IO.File.OpenRead(path), "text/htnl");
            }
            path = $"{_environment.WebRootPath}/welcome-fragment.{CultureInfo.CurrentCulture.Name}.html";
            if (IO.File.Exists(path))
            {
                return File(IO.File.OpenRead(path), "text/htnl");
            }
            path = $"{_environment.WebRootPath}/welcome-fragment.html";
            if (IO.File.Exists(path))
            {
                return File(IO.File.OpenRead(path), "text/htnl");
            }
            return null;
        }
    }
}
