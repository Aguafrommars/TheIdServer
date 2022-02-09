// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class UserRoleValidator : AbstractValidator<Role>
    {
        public UserRoleValidator(Models.User user, IStringLocalizer localizer)
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage(localizer["The role name is required"]);
            RuleFor(m => m.Name).IsUnique(user.Roles).WithMessage(localizer["The role must be unique"]);
        }
    }
}