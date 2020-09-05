// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Models;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class GoogleOptionsValidator : AbstractValidator<GoogleOptions>
    {
        public GoogleOptionsValidator(ExternalProvider _)
        {
            RuleFor(m => m.ClientId).NotEmpty().WithMessage("Client Id is required.");
            RuleFor(m => m.ClientSecret).NotEmpty().WithMessage("Client Secret is required.");
        }
    }
}
