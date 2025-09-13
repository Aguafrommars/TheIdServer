// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Aguacongas.IdentityServer.Store.Entity;
using Models = Aguacongas.TheIdServer.BlazorApp.Models;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ExternalClaimTransformationValidator : AbstractValidator<ExternalClaimTransformation>
    {
        public ExternalClaimTransformationValidator(Models.ExternalProvider externalProvider, IStringLocalizer localizer)
        {
            RuleFor(m => m.FromClaimType).NotEmpty().WithMessage(localizer["The from claim tyoe is required."]);
            RuleFor(m => m.ToClaimType).NotEmpty().WithMessage(localizer["The to claim tyoe is required."]);
            RuleFor(m => m.FromClaimType).IsUnique(externalProvider.ClaimTransformations).WithMessage(localizer["The from claim type must be unique."]);
        }
    }
}