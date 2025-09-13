// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class WsFederationOptionsValidator : AbstractValidator<WsFederationOptions>
    {
        public WsFederationOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.MetadataAddress).NotEmpty().WithMessage(localizer["Metadata address is required."]);
            RuleFor(m => m.MetadataAddress).Uri().WithMessage(localizer["Metadata address must be a valid uri."]);
            RuleFor(m => m.MetadataAddress).Must((options, value) => !options.RequireHttpsMetadata || value?.ToUpperInvariant().StartsWith("HTTPS") == true)
                .WithMessage(localizer["Metadata address must be a valid HTTPS url when 'required https metadata is true'."]);
            RuleFor(m => m.Wtrealm).NotEmpty().WithMessage(localizer["Wtrealm is required."]);
        }
    }
}
