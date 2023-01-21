// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ApiClaimValidator : AbstractValidator<ApiClaim>
    {
        public ApiClaimValidator(ProtectResource api, IStringLocalizer localizer)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage(localizer["The claim type is required."]);
            RuleFor(m => m.Type).MaximumLength(250).WithMessage(localizer["The claim type cannot exceed 2000 chars."]);
            RuleFor(m => m.Type).IsUnique(api.ApiClaims).WithMessage(localizer["The claim type must be unique."]);
        }
    }
}