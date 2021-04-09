// Project: Aguafrommars/TheIdServer
// Copyright (c) 2021 @Olivier Lefebvre
using FluentValidation;
using FluentValidation.Validators;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Aguacongas.TheIdServer.BlazorApp.Validators
{
    public class UniqueValidator<T> : PropertyValidator<T, string> 
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

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            var editedItem = context.InstanceToValidate;
            var propertyName = context.PropertyName;
            propertyName = propertyName.Substring(propertyName.LastIndexOf('.') + 1);
            var property = typeof(T).GetTypeInfo().GetProperty(propertyName);
            return _items.All(item =>
              item.Equals(editedItem) || property.GetValue(item) as string != value);
        }
    }

}
