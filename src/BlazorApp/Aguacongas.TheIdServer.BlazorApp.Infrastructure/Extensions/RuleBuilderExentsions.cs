// Project: Aguafrommars/TheIdServer
// Copyright (c) 2025 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Validators;
using System.Collections.Generic;

namespace FluentValidation
{
    public static class RuleBuilderExentsions
    {
        public static IRuleBuilderOptions<T, TProperty> IsUnique<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, IEnumerable<T> items)
            where T : class
        {
            return ruleBuilder.SetValidator(new UniqueValidator<T, TProperty>(items));
        }

        public static IRuleBuilderOptions<T, string> Uri<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UriValidator<T>());
        }

    }
}
