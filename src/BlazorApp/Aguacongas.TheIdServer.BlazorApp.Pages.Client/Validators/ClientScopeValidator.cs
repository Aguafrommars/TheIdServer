// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientScopeValidator : AbstractValidator<ClientScope>
    {
        public ClientScopeValidator(Client client, IStringLocalizer localizer)
        {
            RuleFor(m => m.Scope).IsUnique(client.AllowedScopes).WithMessage(localizer["Scopes must be unique."]);
        }
    }
}
