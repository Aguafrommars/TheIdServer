// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class RoleValidator : AbstractValidator<Role>
    {
        public RoleValidator(Role role)
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("The name is required.");
            RuleForEach(m => m.Claims)
                .SetValidator(new RoleClaimValidator(role));
        }
    }
}
