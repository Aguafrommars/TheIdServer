using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.IdentityServer.Store.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{ClientGrantType}" />
    public class ClientGrantTypeValidator : AbstractValidator<ClientGrantType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientGrantTypeValidator"/> class.
        /// </summary>
        public ClientGrantTypeValidator()
        {
            RuleFor(m => m.GrantType).Matches("[^\\s]").WithMessage("The grant type cannot contains space.");
        }
    }
}
