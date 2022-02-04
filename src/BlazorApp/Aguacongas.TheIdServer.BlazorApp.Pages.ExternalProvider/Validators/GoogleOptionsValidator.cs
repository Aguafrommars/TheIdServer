// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class GoogleOptionsValidator : AbstractValidator<GoogleOptions>
    {
        public GoogleOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.ClientId).NotEmpty().WithMessage(localizer["Client Id is required."]);
            RuleFor(m => m.ClientSecret).NotEmpty().WithMessage(localizer["Client Secret is required."]);
        }
    }
}
