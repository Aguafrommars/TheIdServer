using FluentValidation;
using FluentValidation.Internal;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNetCore.Components.Forms
{
    public static class EditContextExtensions
    {
        public static EditContext AddFluentValidation(this EditContext editContext)
        {
            if (editContext == null)
            {
                throw new ArgumentNullException(nameof(editContext));
            }

            var messages = new ValidationMessageStore(editContext);

            editContext.OnValidationRequested +=
                (sender, eventArgs) => ValidateModel((EditContext)sender, messages);

            editContext.OnFieldChanged +=
                (sender, eventArgs) => ValidateField(editContext, messages, eventArgs.FieldIdentifier);

            return editContext;
        }

        private static void ValidateModel(EditContext editContext, ValidationMessageStore messages)
        {
            var validator = GetValidatorForModel(editContext.Model, editContext.Model);
            var validationResults = validator.Validate(editContext.Model);

            messages.Clear();
            foreach (var validationResult in validationResults.Errors.Distinct(CompareError.Instance))
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
        {
            var properties = new[] { fieldIdentifier.FieldName };
            var model = fieldIdentifier.Model;
            var context = new ValidationContext(model, new PropertyChain(),
                new MemberNameValidatorSelector(properties));

            var validator = GetValidatorForModel(editContext.Model, model);
            var validationResults = validator.Validate(context);

            messages.Clear(fieldIdentifier);
            foreach(var result in validationResults.Errors.Distinct(CompareError.Instance).Select(error => error.ErrorMessage))
            {
                messages.Add(fieldIdentifier, result);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static IValidator GetValidatorForModel(object entity, object model)
        {
            var abstractValidatorType = typeof(AbstractValidator<>).MakeGenericType(model.GetType());
            var modelValidatorType = Assembly.GetExecutingAssembly()
                .GetTypes().FirstOrDefault(t => t.IsSubclassOf(abstractValidatorType));
            var modelValidatorInstance = (IValidator)Activator.CreateInstance(modelValidatorType, entity);

            return modelValidatorInstance;
        }

        class CompareError : IEqualityComparer<ValidationFailure>
        {
            public static readonly CompareError Instance = new CompareError();
            public bool Equals(ValidationFailure x, ValidationFailure y)
            {
                return x.ErrorMessage == y.ErrorMessage;
            }

            public int GetHashCode(ValidationFailure obj)
            {
                return -1;
            }
        }
    }
}
