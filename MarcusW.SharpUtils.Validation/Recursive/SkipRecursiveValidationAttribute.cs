using System;

namespace MarcusW.SharpUtils.Validation.Recursive
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SkipRecursiveValidationAttribute : Attribute { }
}
