// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class TwitterOptionsValidator : AbstractValidator<TwitterOptions>
    {
        public TwitterOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.ConsumerKey).NotEmpty().WithMessage(localizer["Consumer Key is required."]);
            RuleFor(m => m.ConsumerSecret).NotEmpty().WithMessage(localizer["Consumer Secret is required."]);
        }
    }
}