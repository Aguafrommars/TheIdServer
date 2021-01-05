// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ApiPropertyValidator : AbstractValidator<ApiProperty>
    {
        public ApiPropertyValidator(ProtectResource api, IStringLocalizer localizer)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage(localizer["The api property key is required."]);
            RuleFor(m => m.Key).MaximumLength(250).WithMessage(localizer["The api property key cannot exceed 250 chars."]);
            RuleFor(m => m.Key).IsUnique(api.Properties).WithMessage(localizer["The api property key must be unique."]);
            RuleFor(m => m.Value).NotEmpty().WithMessage(localizer["The api property value is required."]);
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage(localizer["The api property value cannot exceed 2000 chars."]);
        }
    }
}