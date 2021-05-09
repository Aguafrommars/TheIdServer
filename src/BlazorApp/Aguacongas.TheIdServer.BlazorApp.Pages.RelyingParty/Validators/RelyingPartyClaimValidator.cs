// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class RelyingPartyClaimValidator : AbstractValidator<RelyingPartyClaimMapping>
    {
        public RelyingPartyClaimValidator(RelyingParty relyingParty, IStringLocalizer localizer)
        {
            RuleFor(m => m.FromClaimType).NotEmpty().WithMessage(localizer["The from claim type is required."]);
            RuleFor(m => m.ToClaimType).NotEmpty().WithMessage(localizer["The to claim type is required."]);
            RuleFor(m => m.ToClaimType).Must(v => Uri.TryCreate(v, UriKind.Absolute, out Uri _))
                .WithMessage(localizer["The to claim type must be an URI."]);
            RuleFor(m => m.FromClaimType).IsUnique(relyingParty.ClaimMappings).WithMessage(localizer["The from claim type must be unique."]);
        }
    }
}