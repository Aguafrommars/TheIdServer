// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientResourceUriValidator : EntityResourceValidator<ClientLocalizedResource>
    {
        public ClientResourceUriValidator(ILocalizable<ClientLocalizedResource> model, EntityResourceKind kind, IStringLocalizer localizer) : base(model, kind, localizer)
        {
            RuleFor(m => m.Value).Uri().WithMessage(m => localizer["{0} is not a valid uri.", m.Value]);
        }
    }
}
