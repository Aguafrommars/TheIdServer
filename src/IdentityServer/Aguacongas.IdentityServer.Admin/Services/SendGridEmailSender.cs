// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Admin.Services
{
    /// <summary>
    /// SendGrid email sender
    /// </summary>
    public class SendGridEmailSender
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendGridEmailSender"/> class.
        /// </summary>
        /// <param name="optionsAccessor">The options accessor.</param>
        public SendGridEmailSender(IOptions<SendGridOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        /// <value>
        /// The options.
        /// </value>
        public SendGridOptions Options { get; } //set only via Secret Manager

        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <returns></returns>
        public Task SendEmailAsync(Email email)
        {
            return Execute(Options.SendGridKey, email.Subject, email.Message, email.Addresses);
        }

        private Task Execute(string apiKey, string subject, string message, IEnumerable<string> addresseList)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.From, Options.SendGridUser),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };

            foreach(var address in addresseList)
            {
                msg.AddTo(new EmailAddress(address));
            }
            

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(Options.EnableClickTracking, Options.EnableClickTracking);

            return client.SendEmailAsync(msg);
        }
    }
}