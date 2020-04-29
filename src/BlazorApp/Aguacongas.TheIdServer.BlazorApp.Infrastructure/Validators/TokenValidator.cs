using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class TokenValidator : AbstractValidator<Token>
    {
        public TokenValidator(Client _)
        {
            RuleFor(m => m.ValueString)
                .Matches(Token.RegulatExpression)
                .WithMessage("The token expression doesn't match a valid format. You can use the forms d.hh:mm:ss, hh.mm:ss, mm:ss, a number of days (365d), a number of hours (12h), a number of minutes (30m), a number of second");
        }
    }
}
