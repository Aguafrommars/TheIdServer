// Project: Aguafrommars/TheIdServer
// Copyright (c) 2022 @Olivier Lefebvre
using FluentValidation;
using FluentValidation.Validators;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UriValidator<T> : PropertyValidator<T, string>
    {
        [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "Fine like this.")]
        private static readonly Regex _urlRegex = new Regex("^(http|https)://.+");

        public override string Name => $"UriValidatorOf{typeof(T).Name}";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} is not a valid uri.";
        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return _urlRegex.IsMatch(value);
            }
            return true;
        }
    }
}
