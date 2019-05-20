using System;

namespace MarcusW.SharpUtils.AspNetCore.Validation
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SkipRecursiveValidationAttribute : Attribute { }
}
