// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
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
                .WithMessage(localizer["The to claim type must be an URI."])
                .When(m => relyingParty.TokenType == "urn:oasis:names:tc:SAML:1.0:assertion");
            RuleFor(m => m.FromClaimType).IsUnique(relyingParty.ClaimMappings).WithMessage(localizer["The from claim type must be unique."]);
        }
    }
}