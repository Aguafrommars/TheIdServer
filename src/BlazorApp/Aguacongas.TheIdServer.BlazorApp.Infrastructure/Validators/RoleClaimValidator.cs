﻿// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class RoleClaimValidator : AbstractValidator<RoleClaim>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validator constructor requires a parameter.")]
        public RoleClaimValidator(Models.Role role)
        {
            RuleFor(m => m.ClaimType).NotEmpty().WithMessage("The claim type is required.");
            RuleFor(m => m.ClaimType).MaximumLength(250).WithMessage("The claim type cannot exceed 250 chars.");
            RuleFor(m => m.ClaimValue).NotEmpty().WithMessage("The claim value is required.");
        }
    }
}