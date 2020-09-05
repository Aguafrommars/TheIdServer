// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ClientPropertyValidator : AbstractValidator<ClientProperty>
    {
        public ClientPropertyValidator(Client client)
        {
            RuleFor(m => m.Key).NotEmpty().WithMessage("The client property key is required.");
            RuleFor(m => m.Key).MaximumLength(250).WithMessage("The client property key cannot exceed 250 chars.");
            RuleFor(m => m.Key).IsUnique(client.Properties).WithMessage("The client property key must be unique.");
            RuleFor(m => m.Value).NotEmpty().WithMessage("The client property value is required.");
            RuleFor(m => m.Value).MaximumLength(2000).WithMessage("The client property value cannot exceed 2000 chars.");
        }
    }
}