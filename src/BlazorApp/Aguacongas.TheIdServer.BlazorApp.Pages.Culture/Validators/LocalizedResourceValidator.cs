// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class LocalizedResourceValidator : AbstractValidator<LocalizedResource>
    {
        public LocalizedResourceValidator(Culture culture, IStringLocalizer localizer)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage(localizer["The key is required."]);
            RuleFor(m => m.Key).IsUnique(culture.Resources).WithMessage(localizer["The key must be unique."]);
        }
    }
}