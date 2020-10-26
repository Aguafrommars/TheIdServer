// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class FacebookOptionsValidator : AbstractValidator<FacebookOptions>
    {
        public FacebookOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.AppId).NotEmpty().WithMessage(localizer["Client Id is required."]);
            RuleFor(m => m.AppSecret).NotEmpty().WithMessage(localizer["Client Secret is required."]);
        }
    }
}