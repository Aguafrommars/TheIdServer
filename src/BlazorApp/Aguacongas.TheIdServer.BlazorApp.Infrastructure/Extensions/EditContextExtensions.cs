using Aguacongas.IdentityServer.Store.Entity;
using Aguacongas.TheIdServer.BlazorApp.Validators;
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

            var validationResults = validator.Validate(CreateValidationContext(editContext.Model));

            messages.Clear();
            foreach (var validationResult in validationResults.Errors.Distinct(CompareError.Instance))
            {
                messages.Add(editContext.Field(validationResult.PropertyName), validationResult.ErrorMessage);
            }

            editContext.NotifyValidationStateChanged();
        }

        private static void ValidateField(EditContext editContext, ValidationMessageStore messages, in FieldIdentifier fieldIdentifier)
        {
            var context = CreateValidationContext(fieldIdentifier);

            var validator = GetValidatorForModel(editContext.Model, fieldIdentifier.Model);
            if (validator == null)
            {
                return;
            }

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
            if (model is IEntityResource resource)
            {
                var entityValidatorType = typeof(EntityResourceValidator<>).MakeGenericType(model.GetType());              
                return (IValidator)Activator.CreateInstance(entityValidatorType, entity, resource.ResourceKind);
            }
            var abstractValidatorType = typeof(AbstractValidator<>).MakeGenericType(model.GetType());
            var modelValidatorType = Assembly.GetExecutingAssembly()
                .GetTypes().FirstOrDefault(t => t.IsSubclassOf(abstractValidatorType));
            if (modelValidatorType == null)
            {
                return null;
            }

            var modelValidatorInstance = (IValidator)Activator.CreateInstance(modelValidatorType, entity);
            return modelValidatorInstance;
        }

        private static IValidationContext CreateValidationContext(object model)
        {
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(model.GetType());
            return Activator.CreateInstance(validationContextType, model) as IValidationContext;
        }

        private static IValidationContext CreateValidationContext(FieldIdentifier fieldIdentifier)
        {
            var properties = new[] { fieldIdentifier.FieldName };
            var model = fieldIdentifier.Model;
            var validationContextType = typeof(ValidationContext<>).MakeGenericType(model.GetType());
            return Activator.CreateInstance(validationContextType, model, new PropertyChain(), new MemberNameValidatorSelector(properties)) as IValidationContext;
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
