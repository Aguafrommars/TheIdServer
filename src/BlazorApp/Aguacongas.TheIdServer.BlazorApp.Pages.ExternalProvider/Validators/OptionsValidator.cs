// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class OptionsValidator : AbstractValidator<object>
    {
        private readonly ExternalProvider _provider;
        private readonly IStringLocalizer _localizer;

        public OptionsValidator(ExternalProvider provider, IStringLocalizer localizer)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        public override ValidationResult Validate(ValidationContext<object> context)
        {
            var options = _provider.Options;
            var optionsType = options.GetType();
            var result = optionsType.Name switch
            {
                nameof(GoogleOptions) => new GoogleOptionsValidator(_provider, _localizer).Validate(options as GoogleOptions),
                nameof(FacebookOptions) => new FacebookOptionsValidator(_provider, _localizer).Validate(options as FacebookOptions),
                nameof(OAuthOptions) => new OAuthOptionsValidator(_provider, _localizer).Validate(options as OAuthOptions),
                nameof(MicrosoftAccountOptions) => new OAuthOptionsValidator(_provider, _localizer).Validate(options as MicrosoftAccountOptions),
                nameof(OpenIdConnectOptions) => new OpenIdConnectOptionsValidator(_provider, _localizer).Validate(options as OpenIdConnectOptions),
                nameof(TwitterOptions) => new TwitterOptionsValidator(_provider, _localizer).Validate(options as TwitterOptions),
                nameof(WindowsOptions) => new WindowsOptionsValidator(_provider, _localizer).Validate(options as WindowsOptions),
                nameof(WsFederationOptions) => new WsFederationOptionsValidator(_provider, _localizer).Validate(options as WsFederationOptions),
                _ => base.Validate(context),
            };

            return result;
        }

        
    }
}