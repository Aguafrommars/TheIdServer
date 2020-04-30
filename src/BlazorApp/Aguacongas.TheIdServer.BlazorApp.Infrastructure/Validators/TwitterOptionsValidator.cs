using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class TwitterOptionsValidator : AbstractValidator<TwitterOptions>
    {
        public TwitterOptionsValidator(ExternalProvider _)
        {
            RuleFor(m => m.ConsumerKey).NotEmpty().WithMessage("Consumer Key is required.");
            RuleFor(m => m.ConsumerSecret).NotEmpty().WithMessage("Consumer Secret is required.");
        }
    }
}