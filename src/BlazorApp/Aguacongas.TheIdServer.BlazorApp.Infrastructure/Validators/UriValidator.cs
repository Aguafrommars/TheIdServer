// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UriValidator : PropertyValidator
    {
        private static readonly Regex _urlRegex = new Regex("^(http|https)://.+");
        public UriValidator() : base("{PropertyName} is not a valid uri.")
        {
            
        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            var value = context.PropertyValue as string;
            if (!string.IsNullOrEmpty(value))
            {
                return _urlRegex.IsMatch(value);
            }
            return true;
        }
    }
}
