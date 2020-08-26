// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using System.Diagnostics.CodeAnalysis;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class ApiSecretValidator : AbstractValidator<ApiSecret>
    {
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Our validator construtor requires a parameter.")]
        public ApiSecretValidator(ProtectResource api)
        {
            RuleFor(m => m.Type).NotEmpty().WithMessage("The secret type is required.");
            RuleFor(m => m.Value).NotEmpty().WithMessage("The secret value is required.");
        }
    }
}
