// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class OpenIdConnectOptionsValidator : AbstractValidator<OpenIdConnectOptions>
    {
        public OpenIdConnectOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.Authority).NotEmpty().WithMessage(localizer["Authority is required."]);
            RuleFor(m => m.Authority).Uri().WithMessage(localizer["Authority must be a valid uir."]);
            RuleFor(m => m.ClientId).NotEmpty().WithMessage(localizer["Client Id is required."]);
        }
    }
}