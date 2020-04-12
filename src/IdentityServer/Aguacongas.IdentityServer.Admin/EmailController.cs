using Aguacongas.IdentityServer.Admin.Models;
using Aguacongas.IdentityServer.Admin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin
{
    /// <summary>
    /// Email controller
    /// </summary>
    /// <seealso cref="Controller" />
    [Produces("application/json")]
    [Route("[controller]")]
    public class EmailController : Controller
    {
        private SendGridEmailSender _sender;

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
        [Authorize(Policy = "Is4-Writer")]
        public Task SendEmail([FromBody] Email email)
             => _sender.SendEmailAsync(email);
    }
}
