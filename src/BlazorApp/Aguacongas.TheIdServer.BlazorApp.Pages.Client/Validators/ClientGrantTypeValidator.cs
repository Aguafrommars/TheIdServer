// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{ClientGrantType}" />
    public partial class ClientGrantTypeValidator : AbstractValidator<ClientGrantType>
    {
        [GeneratedRegex("\\s", RegexOptions.Compiled)]
        private static partial Regex _regex();
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientGrantTypeValidator"/> class.
        /// </summary>
        public ClientGrantTypeValidator(Client client, IStringLocalizer localizer)
        {
            RuleFor(m => m.GrantType).Must(g => g == null || !_regex().IsMatch(g)).WithMessage(localizer["The grant type cannot contains space."]);
            RuleFor(m => m.GrantType).IsUnique(client.AllowedGrantTypes).WithMessage(localizer["The grant type must be unique."]);
            RuleFor(m => m.GrantType).Must(g => (g != "hybrid" && g != "authorization_code") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "implicit"))
                .WithMessage(g => localizer["'{0}' cannot be added to a client with grant type '{1}'.", GetGrantTypeName(g.GrantType), GetGrantTypeName("implicit")]);
            RuleFor(m => m.GrantType).Must(g => (g != "implicit" && g != "authorization_code") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "hybrid"))
                .WithMessage(g => localizer["'{0}' cannot be added to a client with grant type '{1}'.", GetGrantTypeName(g.GrantType), GetGrantTypeName("hybrid")]);
            RuleFor(m => m.GrantType).Must(g => (g != "implicit" && g != "hybrid") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "authorization_code"))
                .WithMessage(g => localizer["'{0}' cannot be added to a client with grant type '{1}'.", GetGrantTypeName(g.GrantType), GetGrantTypeName("authorization_code")]);
        }

        private static string GetGrantTypeName(string grantType)
            => GrantTypes.GetGrantTypeName(grantType);
    }
}
