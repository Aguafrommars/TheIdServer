// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(User user, IStringLocalizer localizer)
        {
            RuleFor(m => m.UserName).NotEmpty().WithMessage(localizer["The name is required"]);
            RuleForEach(m => m.Claims).SetValidator(new UserClaimValidator(user, localizer));
            RuleForEach(m => m.Roles)
                .Where(m => m.Name != null)
                .SetValidator(new UserRoleValidator(user, localizer));
        }
    }
}
