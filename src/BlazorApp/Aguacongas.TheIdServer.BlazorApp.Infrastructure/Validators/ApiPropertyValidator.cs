// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ApiPropertyValidator : AbstractValidator<ApiProperty>
    {
        public ApiPropertyValidator(ProtectResource api)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage("The api property key is required.");
            RuleFor(m => m.Key).MaximumLength(250).WithMessage("The api property key cannot exceed 250 chars.");
            RuleFor(m => m.Key).IsUnique(api.Properties).WithMessage("The api property key must be unique.");
            RuleFor(m => m.Value).NotEmpty().WithMessage("The api property value is required.");
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage("The api property value cannot exceed 2000 chars.");
        }
    }
}