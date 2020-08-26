// Project: Aguafrommars/TheIdServer
// Copyright (c) 2020 @Olivier Lefebvre
using Aguacongas.TheIdServer.BlazorApp.Validators;
using System.Collections.Generic;

namespace FluentValidation
{
    public static class RuleBuilderExentsions
    {
        public static IRuleBuilderOptions<TItem, TProperty> IsUnique<TItem, TProperty>(this IRuleBuilder<TItem, TProperty> ruleBuilder, IEnumerable<TItem> items)
            where TItem : class
        {
            return ruleBuilder.SetValidator(new UniqueValidator<TItem>(items));
        }

        public static IRuleBuilderOptions<T, string> Uri<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.SetValidator(new UriValidator());
        }

    }
}
