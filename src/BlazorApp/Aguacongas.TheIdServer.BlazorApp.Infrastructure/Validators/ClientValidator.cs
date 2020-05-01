using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
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
        public ClientValidator(Client client)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("The id is required.");
            RuleFor(m => m.ClientClaimsPrefix).MaximumLength(250).WithMessage("The claim prefix cannot exceed 250 char.");
            RuleFor(m => m.PairWiseSubjectSalt).MaximumLength(250).WithMessage("The subject salt cannot exceed 250 char.");
            RuleFor(m => m.UserCodeType).MaximumLength(100).WithMessage("The type of user code cannot exceed 100 char.");
            RuleFor(m => m.ProtocolType).NotEmpty().WithMessage("The type of protocol is required");
            RuleFor(m => m.ProtocolType).MaximumLength(200).WithMessage("The type of protocol cannot exceed 200 char.");
            RuleFor(m => m.ClientUri).MaximumLength(2000).WithMessage("The url cannot exceed 2000 char.");
            RuleFor(m => m.ClientUri).Uri().WithMessage("The url is not valid.");
            RuleFor(m => m.LogoUri).MaximumLength(2000).WithMessage("The logo url cannot exceed 2000 char.");
            RuleFor(m => m.LogoUri).Uri().WithMessage("The logo url is not valid.");
            RuleFor(m => m.FrontChannelLogoutUri).MaximumLength(2000).WithMessage("The front channel logout url cannot exceed 2000 char.");
            RuleFor(m => m.FrontChannelLogoutUri).Uri().WithMessage("The front channel logout url is not valid.");
            RuleFor(m => m.FrontChannelLogoutUri).Must(u => !client.FrontChannelLogoutSessionRequired || !string.IsNullOrEmpty(u))
                .WithMessage("The front channel logout url is required.");
            RuleFor(m => m.BackChannelLogoutUri).MaximumLength(2000).WithMessage("The back channel logout url cannot exceed 2000 char.");
            RuleFor(m => m.BackChannelLogoutUri).Uri().WithMessage("The back logout url is not valid.");
            RuleFor(m => m.BackChannelLogoutUri).Must(u => !client.BackChannelLogoutSessionRequired || !string.IsNullOrEmpty(u))
                .WithMessage("The back channel logout url is required.");
            RuleFor(m => m.ClientName).MaximumLength(200).WithMessage("The name cannot exceed 200 char.");
            RuleFor(m => m.ClientName).Must(n => !client.RequireConsent || 
                (client.RequireConsent && !string.IsNullOrEmpty(n))).WithMessage("The name is required.");
            RuleFor(m => m.Description).MaximumLength(200).WithMessage("The description canoot exceed 1000 char.");
            RuleForEach(m => m.AllowedGrantTypes)
                .Where(m => m.GrantType != null)
                .SetValidator(new ClientGrantTypeValidator(client));
            RuleFor(m => m.AllowedGrantTypes).Must(g => g.Any(g => !string.IsNullOrEmpty(g.GrantType)))
                .WithMessage("The client should contain at least one grant type.");
            RuleForEach(m => m.RedirectUris).SetValidator(new ClientRedirectUriValidator(client));
            RuleForEach(m => m.ClientSecrets).SetValidator(new ClientSecretValidator(client));
            RuleForEach(m => m.Properties).SetValidator(new ClientPropertyValidator(client));
            RuleForEach(m => m.AllowedScopes)
                .Where(m => m.Scope != null)
                .SetValidator(new ClientScopeValidator(client));

        }
    }
}
