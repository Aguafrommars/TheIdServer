// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class RelyingPartyValidator : AbstractValidator<RelyingParty>
    {
        public RelyingPartyValidator(RelyingParty _, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The id is required."]);
            RuleFor(m => m.Description).MaximumLength(2000).WithMessage(localizer["The description cannot exceed 2000 chars."]);
            RuleFor(m => m.DigestAlgorithm).NotEmpty().WithMessage(localizer["The digest algorithm is required"]);
            RuleFor(m => m.SamlNameIdentifierFormat).NotEmpty().WithMessage(localizer["The name identifier is required"]);
            RuleFor(m => m.TokenType).NotEmpty().WithMessage(localizer["The token type is required"]);
            RuleForEach(m => m.ClaimMappings)
                .Where(m => m.FromClaimType != null)
                .SetValidator(m => new RelyingPartyClaimValidator(m, localizer));
        }
    }
}