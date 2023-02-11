// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using FluentValidation;
using FluentValidation.Validators;
using System.Text.RegularExpressions;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public partial class UriValidator<T> : PropertyValidator<T, string>
    {
        [GeneratedRegex("^.+://.+")]
        private static partial Regex _urlRegex();

        public override string Name => $"UriValidatorOf{typeof(T).Name}";

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} is not a valid uri.";
        public override bool IsValid(ValidationContext<T> context, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return _urlRegex().IsMatch(value);
            }
            return true;
        }
    }
}
