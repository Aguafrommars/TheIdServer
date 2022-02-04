// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class EntityResourceValidator<T>: AbstractValidator<T>  where T: class, IEntityResource
    {
        public EntityResourceValidator(ILocalizable<T> model, EntityResourceKind kind, IStringLocalizer localizer)
        {
            RuleFor(m => m.CultureId).NotEmpty().WithMessage(localizer["The culture is required."]);
            RuleFor(m => m.CultureId).IsUnique(model.Resources.Where(r => r.ResourceKind == kind)).WithMessage(localizer["The culture must be unique."]);
        }
    }
}