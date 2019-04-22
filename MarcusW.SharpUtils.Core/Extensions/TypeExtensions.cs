using System;

namespace MarcusW.SharpUtils.Core.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determines if an instance of the given type can be assigned to the current generic type definition.
        /// </summary>
        /// <param name="genericTypeDefinition">A generic type definition</param>
        /// <param name="givenType">The type that should be checked</param>
        // Credits to James Fraumeni: https://stackoverflow.com/questions/74616/how-to-detect-if-type-is-another-generic-type/1075059#1075059
        public static bool IsGenericTypeDefinitionAssignableFrom(this Type genericTypeDefinition, Type givenType)
        {
            if (genericTypeDefinition == null)
                throw new ArgumentNullException(nameof(genericTypeDefinition));
            if (!genericTypeDefinition.IsGenericTypeDefinition)
                throw new ArgumentException("Type is not a generic type definition.", nameof(genericTypeDefinition));
            if (givenType == null)
                throw new ArgumentNullException(nameof(givenType));

            // Is the given type directly constructed from the generic type definition?
            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericTypeDefinition)
                return true;

            // Is any of the implemented interfaces of the given type constructed from the generic type definition?
            foreach (Type interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == genericTypeDefinition)
                    return true;
            }

            // Get the base type of the given type
            Type baseType = givenType.BaseType;
            if (baseType == null)
                return false;

            // Do the same check for the base type again.
            return IsGenericTypeDefinitionAssignableFrom(genericTypeDefinition, baseType);
        }
    }
}
