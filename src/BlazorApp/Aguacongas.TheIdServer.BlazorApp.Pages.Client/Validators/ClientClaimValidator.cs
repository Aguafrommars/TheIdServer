// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientClaimValidator : AbstractValidator<ClientClaim>
    {
        public ClientClaimValidator(Client client, IStringLocalizer localizer)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage(localizer["The claim type is required."]);
            RuleFor(m => m.Type).Must(v => Uri.TryCreate(v, UriKind.Absolute, out Uri _))
                .WithMessage(localizer["The claim type must be an URI."])
                .When(m => client.ProtocolType == Pages.Client.Client.WSFED);
            RuleFor(m => m.Type).MaximumLength(250).WithMessage(localizer["The claim type cannot exceed 250 chars."]);
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage(localizer["The claim value cannot exceed 2000 chars."]);
        }
    }
}