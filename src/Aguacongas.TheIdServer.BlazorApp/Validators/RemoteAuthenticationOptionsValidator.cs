using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using FluentValidation.Results;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class RemoteAuthenticationOptionsValidator : AbstractValidator<RemoteAuthenticationOptions>
    {
        private readonly ExternalProvider _provider;

        public RemoteAuthenticationOptionsValidator(ExternalProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override ValidationResult Validate(ValidationContext<RemoteAuthenticationOptions> context)
        {
            var options = _provider.Options;
            var optionsType = options.GetType();
            return optionsType.Name switch
            {
                nameof(GoogleOptions) => new GoogleOptionsValidator(_provider).Validate(options as GoogleOptions),
                nameof(FacebookOptions) => new FacebookOptionsValidator(_provider).Validate(options as FacebookOptions),
                nameof(OAuthOptions) => new OAuthOptionsValidator(_provider).Validate(options as OAuthOptions),
                nameof(MicrosoftAccountOptions) => new OAuthOptionsValidator(_provider).Validate(options as OAuthOptions),
                nameof(OpenIdConnectOptions) => new OpenIdConnectOptionsValidator(_provider).Validate(options as OpenIdConnectOptions),
                nameof(TwitterOptions) => new TwitterOptionsValidator(_provider).Validate(options as TwitterOptions),
                _ => base.Validate(context),
            };
        }
    }
}