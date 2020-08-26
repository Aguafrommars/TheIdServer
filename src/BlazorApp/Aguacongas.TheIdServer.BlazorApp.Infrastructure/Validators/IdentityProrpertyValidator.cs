// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class IdentityProrpertyValidator : AbstractValidator<IdentityProperty>
    {
        public IdentityProrpertyValidator(IdentityResource identity)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage("The identity property key is required.");
            RuleFor(m => m.Key).MaximumLength(250).WithMessage("The identity property key cannot exceed 250 chars.");
            RuleFor(m => m.Key).IsUnique(identity.Properties).WithMessage("The identity property key must be unique.");
            RuleFor(m => m.Value).NotEmpty().WithMessage("The identity property value is required.");
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage("The identity property value cannot exceed 2000 chars.");
        }
    }
}