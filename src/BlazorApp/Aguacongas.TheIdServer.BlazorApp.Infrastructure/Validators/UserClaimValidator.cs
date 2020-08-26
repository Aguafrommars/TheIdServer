﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class UserClaimValidator : AbstractValidator<UserClaim>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validator required a parameter.")]
        public UserClaimValidator(Models.User user)
        {
            RuleFor(m => m.ClaimType).NotEmpty().WithMessage("The claim type is required.");
            RuleFor(m => m.ClaimValue).NotEmpty().WithMessage("The claim value is required.");
        }
    }
}