// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="AbstractValidator{Client}" />
    public class CutlureValidator : AbstractValidator<Culture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientValidator"/> class.
        /// </summary>
        public CutlureValidator(Culture culture, IStringLocalizer localizer)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage(localizer["The name is required."]);
            RuleForEach(m => m.Resources)
                .SetValidator(new LocalizedResourceValidator(culture, localizer));

        }
    }
}
