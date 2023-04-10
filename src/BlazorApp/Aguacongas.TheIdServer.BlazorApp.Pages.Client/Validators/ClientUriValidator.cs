// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using FluentValidation;
using Microsoft.Extensions.Localization;
using Entity = Aguacongas.IdentityServer.Store.Entity;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientUriValidator : AbstractValidator<Entity.ClientUri>
    {
        public ClientUriValidator(Entity.Client _, IStringLocalizer localizer)
        {
            RuleFor(m => m.Uri).MaximumLength(2000).WithMessage(localizer["An url cannot exceed 2000 char."]);
            RuleFor(m => m.Uri).Uri().WithMessage((c, v) => $"The url '{v}' is not valid.");
        }
    }
}
