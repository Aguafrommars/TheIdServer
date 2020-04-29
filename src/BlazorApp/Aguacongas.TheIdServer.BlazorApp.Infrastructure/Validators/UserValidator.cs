using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator(User user)
        {
            RuleFor(m => m.UserName).NotEmpty().WithMessage("The name is required");
            RuleForEach(m => m.Claims).SetValidator(new UserClaimValidator(user));
            RuleForEach(m => m.Roles)
                .Where(m => m.Name != null)
                .SetValidator(new UserRoleValidator(user));
        }
    }
}
