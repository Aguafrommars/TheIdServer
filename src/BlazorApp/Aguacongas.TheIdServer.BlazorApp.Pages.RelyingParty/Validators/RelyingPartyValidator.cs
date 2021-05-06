// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
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
            RuleForEach(m => m.ClaimMappings)
                .Where(m => m.FromClaimType != null)
                .SetValidator(m => new RelyingPartyClaimValidator(m, localizer));
        }
    }
}