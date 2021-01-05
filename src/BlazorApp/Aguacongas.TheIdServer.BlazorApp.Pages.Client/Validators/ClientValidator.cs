// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{Client}" />
    public class ClientValidator : AbstractValidator<Client>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientValidator"/> class.
        /// </summary>
        public ClientValidator(Client client, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The id is required."]);
            RuleFor(m => m.ClientClaimsPrefix).MaximumLength(250).WithMessage(localizer["The claim prefix cannot exceed 250 char."]);
            RuleFor(m => m.PairWiseSubjectSalt).MaximumLength(250).WithMessage(localizer["The subject salt cannot exceed 250 char."]);
            RuleFor(m => m.UserCodeType).MaximumLength(100).WithMessage(localizer["The type of user code cannot exceed 100 char."]);
            RuleFor(m => m.ProtocolType).NotEmpty().WithMessage(localizer["The type of protocol is required"]);
            RuleFor(m => m.ProtocolType).MaximumLength(200).WithMessage(localizer["The type of protocol cannot exceed 200 char."]);
            RuleFor(m => m.ClientUri).MaximumLength(2000).WithMessage(localizer["The url cannot exceed 2000 char."]);
            RuleFor(m => m.ClientUri).Uri().WithMessage(localizer["The url is not valid."]);
            RuleFor(m => m.LogoUri).MaximumLength(2000).WithMessage(localizer["The logo url cannot exceed 2000 char."]);
            RuleFor(m => m.LogoUri).Uri().WithMessage(localizer["The logo url is not valid."]);
            RuleFor(m => m.PolicyUri).MaximumLength(2000).WithMessage(localizer["The policy url cannot exceed 2000 char."]);
            RuleFor(m => m.PolicyUri).Uri().WithMessage(localizer["The policy url is not valid."]);
            RuleFor(m => m.TosUri).MaximumLength(2000).WithMessage(localizer["The terms of service url cannot exceed 2000 char."]);
            RuleFor(m => m.TosUri).Uri().WithMessage(localizer["The terms of service url is not valid."]);
            RuleFor(m => m.FrontChannelLogoutUri).MaximumLength(2000).WithMessage(localizer["The front channel logout url cannot exceed 2000 char."]);
            RuleFor(m => m.FrontChannelLogoutUri).Uri().WithMessage(localizer["The front channel logout url is not valid."]);
            RuleFor(m => m.FrontChannelLogoutUri).Must(u => !client.FrontChannelLogoutSessionRequired || !string.IsNullOrEmpty(u))
                .WithMessage(localizer["The front channel logout url is required."]);
            RuleFor(m => m.BackChannelLogoutUri).MaximumLength(2000).WithMessage(localizer["The back channel logout url cannot exceed 2000 char."]);
            RuleFor(m => m.BackChannelLogoutUri).Uri().WithMessage(localizer["The back logout url is not valid."]);
            RuleFor(m => m.BackChannelLogoutUri).Must(u => !client.BackChannelLogoutSessionRequired || !string.IsNullOrEmpty(u))
                .WithMessage(localizer["The back channel logout url is required."]);
            RuleFor(m => m.ClientName).MaximumLength(200).WithMessage(localizer["The name cannot exceed 200 char."]);
            RuleFor(m => m.ClientName).Must(n => !client.RequireConsent || 
                (client.RequireConsent && !string.IsNullOrEmpty(n))).WithMessage(localizer["The name is required."]);
            RuleFor(m => m.Description).MaximumLength(200).WithMessage(localizer["The description canoot exceed 1000 char."]);
            RuleForEach(m => m.AllowedGrantTypes)
                .Where(m => m.GrantType != null)
                .SetValidator(new ClientGrantTypeValidator(client, localizer));
            RuleFor(m => m.AllowedGrantTypes).Must(g => g.Any(g => !string.IsNullOrEmpty(g.GrantType)))
                .WithMessage(localizer["The client should contain at least one grant type."]);
            RuleForEach(m => m.RedirectUris).SetValidator(new ClientRedirectUriValidator(client, localizer));
            RuleForEach(m => m.ClientSecrets).SetValidator(new ClientSecretValidator(client, localizer));
            RuleForEach(m => m.Properties).SetValidator(new ClientPropertyValidator(client, localizer));
            RuleForEach(m => m.AllowedScopes)
                .Where(m => m.Scope != null)
                .SetValidator(new ClientScopeValidator(client, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.DisplayName)
                .SetValidator(new EntityResourceValidator<ClientLocalizedResource>(client, EntityResourceKind.DisplayName, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.Description)
                .SetValidator(new EntityResourceValidator<ClientLocalizedResource>(client, EntityResourceKind.Description, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.LogoUri)
                .SetValidator(new ClientResourceUriValidator(client, EntityResourceKind.Description, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.ClientUri)
                .SetValidator(new ClientResourceUriValidator(client, EntityResourceKind.Description, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.PolicyUri)
                .SetValidator(new ClientResourceUriValidator(client, EntityResourceKind.Description, localizer));
            RuleForEach(m => m.Resources)
                .Where(m => m.ResourceKind == EntityResourceKind.TosUri)
                .SetValidator(new ClientResourceUriValidator(client, EntityResourceKind.Description, localizer));
        }
    }
}
