// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientClaimValidator : AbstractValidator<ClientClaim>
    {
        public ClientClaimValidator(Client _, IStringLocalizer localizer)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage(localizer["The claim type is required."]);
            RuleFor(m => m.Type).MaximumLength(250).WithMessage(localizer["The claim type cannot exceed 250 chars."]);
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage(localizer["The claim value cannot exceed 2000 chars."]);
        }
    }
}