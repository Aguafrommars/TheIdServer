// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.WsFederation;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.WsFederation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="IActionResult" />
    public class SignInResult : IActionResult
    {
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public WsFederationMessage Message { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SignInResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SignInResult(WsFederationMessage message)
        {
            Message = message;
        }

        /// <summary>
        /// Executes the result operation of the action method asynchronously. This method is called by MVC to process
        /// the result of an action method.
        /// </summary>
        /// <param name="context">The context in which the result is executed. The context information includes
        /// information about the action that was executed and request information.</param>
        /// <returns>
        /// A task that represents the asynchronous execute operation.
        /// </returns>
        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = "text/html";
            var message = Message.BuildFormPost();
            return context.HttpContext.Response.WriteAsync(message);
        }
    }
}
