// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Aguacongas.IdentityServer.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Aguacongas.IdentityServer.Admin.Configuration;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Email controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [ApiRoute("[controller]")]
    public class EmailController : Controller
    {
        private readonly SendGridEmailSender _sender;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailController"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <exception cref="ArgumentNullException">sender</exception>
        public EmailController(SendGridEmailSender sender)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        [HttpPost]
        [Description("Send an email using SendGrid")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
        [Authorize(Policy = SharedConstants.WRITERPOLICY)]
        public Task SendEmail([FromBody] Email email)
        => _sender.SendEmailAsync(email);
    }
}
