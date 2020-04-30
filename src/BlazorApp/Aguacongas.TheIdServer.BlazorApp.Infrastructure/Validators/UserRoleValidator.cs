using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    internal class UserRoleValidator : AbstractValidator<Role>
    {
        public UserRoleValidator(Models.User user)
        {
            RuleFor(m => m.Name).NotEmpty().WithMessage("The role name is required");
            RuleFor(m => m.Name).IsUnique(user.Roles).WithMessage("The role must be unique");
        }
    }
}