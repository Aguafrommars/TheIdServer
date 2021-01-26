// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UniqueValidator<T> : PropertyValidator where T : class
    {
        private readonly IEnumerable<T> _items;

        public UniqueValidator(IEnumerable<T> items)
          : base()
        {
            _items = items;
        }

        protected override string GetDefaultMessageTemplate() => "{PropertyName} must be unique";

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var editedItem = context.InstanceToValidate as T;
            var newValue = context.PropertyValue as string;
            var propertyName = context.PropertyName;
            propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName);
            return _items.All(item =>
              item.Equals(editedItem) || property.GetValue(item) as string != newValue);
        }
    }

}
