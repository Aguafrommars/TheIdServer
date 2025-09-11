// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class UserClaimValidator : AbstractValidator<UserClaim>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validator required a parameter.")]
        public UserClaimValidator(Models.User user, IStringLocalizer localizer)
        {
            RuleFor(m => m.ClaimType).NotEmpty().WithMessage(localizer["The claim type is required."]);
            RuleFor(m => m.ClaimValue).NotEmpty().WithMessage(localizer["The claim value is required."]);
        }
    }
}