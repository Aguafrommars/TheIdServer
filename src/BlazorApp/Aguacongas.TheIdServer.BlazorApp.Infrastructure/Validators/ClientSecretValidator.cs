using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientSecretValidator : AbstractValidator<ClientSecret>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validator construtor requires a parameter.")]
        public ClientSecretValidator(Client client)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("The secret type is required.");
            RuleFor(m => m.Value).NotEmpty().WithMessage($"The secret value is required.");
        }
    }
}
