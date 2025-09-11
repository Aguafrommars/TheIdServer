// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class IdentityProrpertyValidator : AbstractValidator<IdentityProperty>
    {
        public IdentityProrpertyValidator(IdentityResource identity, IStringLocalizer localizer)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage(localizer["The identity property key is required."]);
            RuleFor(m => m.Key).MaximumLength(250).WithMessage(localizer["The identity property key cannot exceed 250 chars."]);
            RuleFor(m => m.Key).IsUnique(identity.Properties).WithMessage(localizer["The identity property key must be unique."]);
            RuleFor(m => m.Value).NotEmpty().WithMessage(localizer["The identity property value is required."]);
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage(localizer["The identity property value cannot exceed 2000 chars."]);
        }
    }
}