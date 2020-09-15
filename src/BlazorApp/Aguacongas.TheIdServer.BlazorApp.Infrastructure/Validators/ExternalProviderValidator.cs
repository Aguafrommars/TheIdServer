// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ExternalProviderValidator: AbstractValidator<ExternalProvider>
    {
        public ExternalProviderValidator(ExternalProvider externalProvider, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The sheme is required."]);
            RuleFor(m => m.DisplayName).NotEmpty().WithMessage(localizer["The display name is required."]);
            RuleFor(m => m.KindName).NotEmpty().WithMessage(localizer["The kind of provider is required."]);
            RuleFor(m => m.Options).SetValidator(p => new RemoteAuthenticationOptionsValidator(externalProvider, localizer));
            RuleForEach(m => m.ClaimTransformations).SetValidator(p => new ExternalClaimTransformationValidator(externalProvider, localizer));
        }
    }
}
