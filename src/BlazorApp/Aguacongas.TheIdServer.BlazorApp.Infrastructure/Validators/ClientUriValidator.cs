using FluentValidation;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientUriValidator : AbstractValidator<Entity.ClientUri>
    {
        public ClientUriValidator(Entity.Client _)
        {
            RuleFor(m => m.Uri).MaximumLength(2000).WithMessage("An url cannot exceed 2000 char.");
            RuleFor(m => m.Uri).Uri().WithMessage((c, v) => $"The url '{v}' is not valid.");
        }
    }
}
