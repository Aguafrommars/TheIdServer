// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{ClientGrantType}" />
    public class ClientGrantTypeValidator : AbstractValidator<ClientGrantType>
    {
        private readonly Regex _regex = new Regex("\\s", RegexOptions.Compiled);
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientGrantTypeValidator"/> class.
        /// </summary>
        public ClientGrantTypeValidator(IdentityServer.Store.Entity.Client client)
        {
            RuleFor(m => m.GrantType).Must(g => g == null || !_regex.IsMatch(g)).WithMessage("The grant type cannot contains space.");
            RuleFor(m => m.GrantType).IsUnique(client.AllowedGrantTypes).WithMessage("The grant type must be unique.");
            RuleFor(m => m.GrantType).Must(g => (g != "hybrid" && g != "authorization_code") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "implicit"))
                .WithMessage(g => $"'{GetGrantTypeName(g.GrantType)}' cannot be added to a client with grant type '{GetGrantTypeName("implicit")}'.");
            RuleFor(m => m.GrantType).Must(g => (g != "implicit" && g != "authorization_code") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "hybrid"))
                .WithMessage(g => $"'{GetGrantTypeName(g.GrantType)}' cannot be added to a client with grant type '{GetGrantTypeName("hybrid")}'.");
            RuleFor(m => m.GrantType).Must(g => (g != "implicit" && g != "hybrid") ||
                !client.AllowedGrantTypes.Any(gt => gt.GrantType == "authorization_code"))
                .WithMessage(g => $"'{GetGrantTypeName(g.GrantType)}' cannot be added to a client with grant type '{GetGrantTypeName("authorization_code")}'.");
        }

        private static string GetGrantTypeName(string grantType)
            => GrantTypes.GetGrantTypeName(grantType);
    }
}
