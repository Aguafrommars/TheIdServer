using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class EntityResourceValidator<T>: AbstractValidator<T>  where T:class, IEntityResource
    {
        public EntityResourceValidator(ILocalizable<T> model, EntityResourceKind kind)
        {
            RuleFor(m => m.CultureId).NotEmpty().WithMessage("The culture is required.");
            RuleFor(m => m.CultureId).IsUnique(model.Resources.Where(r => r.ResourceKind == kind)).WithMessage("The culture must be unique.");
        }
    }
}