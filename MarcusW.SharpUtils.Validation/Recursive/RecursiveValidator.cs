using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MarcusW.SharpUtils.Validation.Recursive
{
    // Based on https://stackoverflow.com/questions/7663501/dataannotations-recursively-validating-an-entire-object-graph/8090614#8090614
    public static class RecursiveValidator
    {
        /// <summary>
        ///     Tests recursively whether the given object instance is valid.
        /// </summary>
        /// <param name="instance">The object instance to test.  It cannot be null.</param>
        /// <param name="validationContext">Describes the object to validate and provides services and context for the validators.</param>
        /// <param name="validationResults">Optional collection to receive <see cref="ValidationResult" />s for the failures.</param>
        /// <returns><c>true</c> if the object is valid, <c>false</c> if any validation errors are encountered.</returns>
        public static bool TryValidateObject(object instance, ValidationContext validationContext, ICollection<ValidationResult> validationResults)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (validationContext == null)
                throw new ArgumentNullException(nameof(validationContext));

            return TryValidateObjectInternal(instance, validationContext, validationResults, new List<object>());
        }

        private static bool TryValidateObjectInternal(object instance,
                                                      ValidationContext validationContext,
                                                      ICollection<ValidationResult> results,
                                                      ICollection<object> validatedObjects)
        {
            // Track previously validated objects to detect loops
            if (validatedObjects.Contains(instance))
                return true;
            validatedObjects.Add(instance);

            // Validate object
            bool result = Validator.TryValidateObject(instance, validationContext, results);

            // Get all contained readable properties that should be validated, too.
            IEnumerable<PropertyInfo> properties = instance.GetType().GetProperties().Where(prop => prop.CanRead
                                                                                                    && prop.GetCustomAttribute<SkipRecursiveValidationAttribute>(false) == null
                                                                                                    && prop.GetIndexParameters().Length == 0);

            // Iterate over contained properties
            foreach (PropertyInfo property in properties)
            {
                // Skip strings and value types (includes primitive types and nullable primitive types)
                Type propertyType = property.PropertyType;
                if (propertyType == typeof(string) || propertyType.IsValueType)
                    continue;

                // Retrieve property value
                var value = property.GetValue(instance);
                if (value == null)
                    continue;

                // Handle enumerable types
                if (value is IEnumerable enumerableValue)
                {
                    int index = 0;
                    foreach (var item in enumerableValue)
                    {
                        if (item != null)
                            ValidateSubInstance(item, $"{property.Name}[{index}].");
                        index++;
                    }
                }
                else
                {
                    ValidateSubInstance(value, $"{property.Name}.");
                }
            }

            void ValidateSubInstance(object subInstance, string memberPrefix)
            {
                // Validate sub instance
                var subValidationContext = new ValidationContext(subInstance, validationContext, validationContext.Items);
                var nestedResults = new List<ValidationResult>();
                if (TryValidateObjectInternal(subInstance, subValidationContext, nestedResults, validatedObjects))
                    return;

                // Validation failed. Unpack validation results.
                result = false;
                if (results != null)
                {
                    foreach (var validationResult in nestedResults)
                        results.Add(new ValidationResult(validationResult.ErrorMessage, validationResult.MemberNames.Select(x => memberPrefix + x)));
                }
            }

            return result;
        }
    }
}
