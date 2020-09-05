// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientScopeValidator : AbstractValidator<ClientScope>
    {
        public ClientScopeValidator(Client client)
        {
            RuleFor(m => m.Scope).IsUnique(client.AllowedScopes).WithMessage("Scopes must be unique.");
        }
    }
}
