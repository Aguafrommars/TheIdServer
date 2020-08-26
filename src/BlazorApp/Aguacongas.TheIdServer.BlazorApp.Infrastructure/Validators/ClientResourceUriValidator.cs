// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientResourceUriValidator : EntityResourceValidator<ClientLocalizedResource>
    {
        public ClientResourceUriValidator(ILocalizable<ClientLocalizedResource> model, EntityResourceKind kind) : base(model, kind)
        {
            RuleFor(m => m.Value).Uri().WithMessage(m => $"{m.Value} is not a valid uri.");
        }
    }
}
