// Project: Aguafrommars/TheIdServer
// Copyright (c) 2023 @Olivier Lefebvre
using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UniqueValidator<T, TProperty> : PropertyValidator<T, TProperty> 
        where T : class
    {
        private readonly IEnumerable<T> _items;

        public override string Name => $"UniqueValidatorOf{typeof(T).Name}";

        public UniqueValidator(IEnumerable<T> items)
          : base()
        {
            _items = items;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) => "{PropertyName} must be unique";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            var editedItem = context.InstanceToValidate;
            var propertyPath = context.PropertyPath;
            propertyPath = propertyPath.Substring(propertyPath.LastIndexOf('.') + 1);
            var property = typeof(T).GetTypeInfo().GetProperty(propertyPath);
            return _items.All(item =>
              item.Equals(editedItem) || !((TProperty)property.GetValue(item)).Equals(value));
        }
    }

}
