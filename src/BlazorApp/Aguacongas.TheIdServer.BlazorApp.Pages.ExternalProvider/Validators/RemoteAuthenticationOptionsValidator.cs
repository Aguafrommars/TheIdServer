// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class RemoteAuthenticationOptionsValidator : AbstractValidator<RemoteAuthenticationOptions>
    {
        private readonly ExternalProvider _provider;
        private readonly IStringLocalizer _localizer;

        public RemoteAuthenticationOptionsValidator(ExternalProvider provider, IStringLocalizer localizer)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public override ValidationResult Validate(ValidationContext<RemoteAuthenticationOptions> context)
        {
            var options = _provider.Options;
            var optionsType = options.GetType();
            return optionsType.Name switch
            {
                nameof(GoogleOptions) => new GoogleOptionsValidator(_provider, _localizer).Validate(options as GoogleOptions),
                nameof(FacebookOptions) => new FacebookOptionsValidator(_provider, _localizer).Validate(options as FacebookOptions),
                nameof(OAuthOptions) => new OAuthOptionsValidator(_provider, _localizer).Validate(options as OAuthOptions),
                nameof(MicrosoftAccountOptions) => new OAuthOptionsValidator(_provider, _localizer).Validate(options as OAuthOptions),
                nameof(OpenIdConnectOptions) => new OpenIdConnectOptionsValidator(_provider, _localizer).Validate(options as OpenIdConnectOptions),
                nameof(WsFederationOptions) => new WsFederationOptionsValidator(_provider, _localizer).Validate(options as WsFederationOptions),
                nameof(TwitterOptions) => new TwitterOptionsValidator(_provider, _localizer).Validate(options as TwitterOptions),
                _ => base.Validate(context),
            };
        }
    }
}