// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.IdentityServer.Store.Entity;
using FluentValidation;

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
        public CutlureValidator(Culture culture)
        {
            RuleFor(m => m.Id).NotEmpty().WithMessage("The name is required.");
            RuleForEach(m => m.Resources)
                .SetValidator(new LocalizedResourceValidator(culture));

        }
    }
}
