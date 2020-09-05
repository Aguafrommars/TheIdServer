// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class LocalizedResourceValidator : AbstractValidator<LocalizedResource>
    {
        public LocalizedResourceValidator(Culture culture)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage("The key is required.");
            RuleFor(m => m.Key).IsUnique(culture.Resources).WithMessage("The key must be unique.");
        }
    }
}