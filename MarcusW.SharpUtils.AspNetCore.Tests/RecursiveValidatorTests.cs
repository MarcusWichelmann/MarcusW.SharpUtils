using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MarcusW.SharpUtils.AspNetCore.Validation;
using Xunit;

namespace MarcusW.SharpUtils.AspNetCore.Tests
{
    public class RecursiveValidatorTests
    {
        [Fact]
        public void ValidatesObjectRecursively()
        {
            var root = new RootClass();
            var validationContext = new ValidationContext(root);
            var validationResults = new List<ValidationResult>();

            // Validate recursively
            Assert.False(RecursiveValidator.TryValidateObject(root, validationContext, validationResults));
            Assert.Equal(6, validationResults.Count);
            Assert.Contains(validationResults, r => r.MemberNames.FirstOrDefault() == nameof(RootClass.SomeString));
            Assert.Contains(validationResults, r => r.MemberNames.FirstOrDefault() == $"{nameof(RootClass.SingleChild)}.{nameof(ChildClass.SomeOtherString)}");
            Assert.Contains(validationResults,
                            r => r.MemberNames.FirstOrDefault() == $"{nameof(RootClass.SingleChild)}.{nameof(ChildClass.NestedItems)}[0].{nameof(ChildItem.SomeOtherOtherString)}");
            Assert.Contains(validationResults,
                            r => r.MemberNames.FirstOrDefault() == $"{nameof(RootClass.SingleChild)}.{nameof(ChildClass.NestedItems)}[1].{nameof(ChildItem.SomeOtherOtherString)}");
            Assert.Contains(validationResults, r => r.MemberNames.FirstOrDefault() == $"{nameof(RootClass.Items)}[0].{nameof(ChildItem.SomeOtherOtherString)}");
            Assert.Contains(validationResults, r => r.MemberNames.FirstOrDefault() == $"{nameof(RootClass.Items)}[1].{nameof(ChildItem.SomeOtherOtherString)}");
        }

        private class RootClass
        {
            [Required]
            public string SomeString { get; } = null;

            public ChildClass SingleChild { get; } = new ChildClass();

            public ChildItem[] Items { get; } = { new ChildItem(), new ChildItem() };

            public ChildItem[] NullEnumerable { get; } = null;

            [SkipRecursiveValidation]
            public IgnoredClass Ignored { get; } = new IgnoredClass();
        }

        private class ChildClass
        {
            [Required]
            public string SomeOtherString { get; } = null;

            public ChildItem[] NestedItems { get; } = { new ChildItem(), new ChildItem() };
        }

        private class ChildItem
        {
            [Required]
            public string SomeOtherOtherString { get; } = null;
        }

        private class IgnoredClass
        {
            [Required]
            public string SomethingElse { get; } = null;
        }
    }
}
