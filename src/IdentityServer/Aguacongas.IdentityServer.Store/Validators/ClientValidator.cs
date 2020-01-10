using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Linq;

namespace Aguacongas.IdentityServer.Store.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{Client}" />
    public class ClientValidator : AbstractValidator<Client>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientValidator"/> class.
        /// </summary>
        public ClientValidator()
        {
            RuleForEach(m => m.AllowedGrantTypes)
                .Must((c, grantType) => c.AllowedGrantTypes.Count(g => g.GrantType == grantType.GrantType) == 1)
                .WithMessage((c, g) => $"Duplicated grant type {g.GrantType}");
            RuleForEach(m => m.AllowedGrantTypes)
                .Must((c, grantType) => (grantType.GrantType != "hybrid" && grantType.GrantType != "authorization_code") ||
                    !c.AllowedGrantTypes.Any(g => g.GrantType == "implicit"))
                .WithMessage((c, g) => $"You cannot add {GetGrantTypeName(g.GrantType)} to a client with {GetGrantTypeName("implicit")} grant type.");
            RuleForEach(m => m.AllowedGrantTypes)
                .Must((c, grantType) => (grantType.GrantType != "hybrid" && grantType.GrantType != "implicit") ||
                    !c.AllowedGrantTypes.Any(g => g.GrantType == "authorization_code"))
                .WithMessage((c, g) => $"You cannot add {GetGrantTypeName(g.GrantType)} to a client with {GetGrantTypeName("authorization_code")} grant type.");
            RuleForEach(m => m.AllowedGrantTypes)
                .Must((c, grantType) => (grantType.GrantType != "authorization_code" && grantType.GrantType != "implicit") ||
                    !c.AllowedGrantTypes.Any(g => g.GrantType == "hybrid"))
                .WithMessage((c, g) => $"You cannot add {GetGrantTypeName(g.GrantType)} to a client with {GetGrantTypeName("hybrid")} grant type.");

            RuleForEach(m => m.AllowedScopes)
                .Must((c, scope) => c.AllowedScopes.Count(s => s.Scope == scope.Scope) == 1)
                .WithMessage((c, s) => $"Duplicated scope {s.Scope}");
            RuleForEach(m => m.Properties)
                .Must((c, property) => c.Properties.Count(p => p.Key == property.Key) == 1)
                .WithMessage((c, p) => $"Duplicated key {p.Key}");
            RuleForEach(m => m.IdentityProviderRestrictions)
                .Must((c, provider) => c.IdentityProviderRestrictions.Count(p => p.Provider == provider.Provider) == 1)
                .WithMessage((c, p) => $"Duplicated provider {p.Provider}");
            RuleForEach(m => m.RedirectUris)
                .Must((c, uri) => c.RedirectUris.Count(u => u.Uri == uri.Uri) == 1)
                .WithMessage((c, u) => $"Duplicated URI {u.Uri}");
        }

        private string GetGrantTypeName(string key)
        {
            return GrantTypes.GetGrantTypeName(key);
        }

    }
}
