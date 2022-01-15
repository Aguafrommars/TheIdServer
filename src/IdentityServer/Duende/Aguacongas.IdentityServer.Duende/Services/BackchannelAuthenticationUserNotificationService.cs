using Aguacongas.IdentityServer.Models;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Aguacongas.IdentityServer.Services
{
    public class BackchannelAuthenticationUserNotificationService : IBackchannelAuthenticationUserNotificationService
    {
        private readonly IIssuerNameService _nameService;
        private readonly IStringLocalizer _localizer;
        private readonly HttpClient _httpClient;
        private readonly IOptions<BackchannelAuthenticationUserNotificationServiceOptions> _options;

        public BackchannelAuthenticationUserNotificationService(IIssuerNameService nameService,
            IStringLocalizer<BackchannelAuthenticationUserNotificationService> localizer,
            HttpClient httpClient, 
            IOptions<BackchannelAuthenticationUserNotificationServiceOptions> options)
        {
            _nameService = nameService ?? throw new ArgumentNullException(nameof(nameService));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task SendLoginRequestAsync(BackchannelUserLoginRequest request)
        {
            var htmlMessage = await CreateMessage(request).ConfigureAwait(false);
            var subject = _localizer["Authorization request"];

            using var content = new StringContent(JsonSerializer.Serialize(new Email
            {
                Addresses = new string[] { request.Subject.FindFirst(c => c.Type == JwtClaimTypes.Email)?.Value },
                Message = htmlMessage,
                Subject = subject
            }), Encoding.UTF8, "application/json");
            using var response = await _httpClient.PostAsync(_options.Value.ApiUrl, content)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        private async Task<string> CreateMessage(BackchannelUserLoginRequest request)
        {
            var client = request.Client;
            var builder = new StringBuilder();
            if (client.LogoUri is not null)
            {
                builder.Append("<div><img src=\"");
                builder.Append(client.LogoUri);
                builder.Append("\"></div>");
            }

            builder.Append(_localizer["{0} is requesting your permission", client.ClientName]);

            if (!string.IsNullOrEmpty(request.BindingMessage))
            {
                builder.Append("<div>");
                builder.Append(_localizer["Verify that this identifier matches what the client is displaying: {0}", request.BindingMessage]);
                builder.Append("</div>");
            }

            builder.Append("<div>");
            builder.Append(_localizer[" Do you wish to continue?", client.ClientName]);
            builder.Append("</div>");

            var issuer = await _nameService.GetCurrentAsync().ConfigureAwait(false);
            if (!issuer.EndsWith('/'))
            {
                issuer += "/";
            }

            builder.Append("<div>");
            builder.Append("<a href =\"");
            builder.Append(issuer);
            builder.Append("ciba/consent?id=");
            builder.Append(request.InternalId);
            builder.Append("\">");
            builder.Append(_localizer["Yes, Continue"]);
            builder.Append("</a>");
            builder.Append("</div>");

            return builder.ToString();
        }
    }
}
