// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class OAuthOptionsValidator : AbstractValidator<OAuthOptions>
    {
        public OAuthOptionsValidator(ExternalProvider _, IStringLocalizer localizer)
        {
            RuleFor(m => m.ClientId).NotEmpty().WithMessage(localizer["Client Id is required."]);
            RuleFor(m => m.AuthorizationEndpoint).NotEmpty().WithMessage(localizer["Authorization endpoint is required."]);
            RuleFor(m => m.AuthorizationEndpoint).Uri().WithMessage(localizer["Authorization endpoint must be a valid uri."]);
            RuleFor(m => m.TokenEndpoint).NotEmpty().WithMessage(localizer["Token endpoint is required."]);
            RuleFor(m => m.TokenEndpoint).Uri().WithMessage(localizer["Token endpoint must be a valid uri."]);
            RuleFor(m => m.UserInformationEndpoint).NotEmpty().WithMessage(localizer["User information endpoint is required."]);
            RuleFor(m => m.UserInformationEndpoint).Uri().WithMessage(localizer["User information endpoint must be a valid uri."]);
        }
    }
}