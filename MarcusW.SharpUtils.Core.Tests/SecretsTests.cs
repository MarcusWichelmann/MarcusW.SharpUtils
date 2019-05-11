using System;
using System.Linq;
using System.Security;
using MarcusW.SharpUtils.Core.Cryptography;
using Xunit;
using Xunit.Sdk;

namespace MarcusW.SharpUtils.Core.Tests
{
    public class SecretsTests
    {
        [Fact]
        public void ThrowsIfTokenLengthNull()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Secrets.GenerateToken(0));
        }

        [Fact]
        public void ThrowsIfPasswordLengthNull()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Secrets.GeneratePassword(0));
        }

        [Fact]
        public void GeneratedTokensAreDifferent()
        {
            byte[] lastToken = null;
            for (int i = 0; i < 100; i++)
            {
                byte[] token = Secrets.GenerateToken(100);
                Assert.Equal(100, token.Length);
                if (lastToken?.SequenceEqual(token) == true)
                    throw new XunitException("Duplicate detected.");
                lastToken = token;
            }
        }

        [Theory]
        [InlineData(PasswordGenerationFlags.Default)]
        [InlineData(PasswordGenerationFlags.WithNumbers)]
        [InlineData(PasswordGenerationFlags.WithLowerCaseChars)]
        [InlineData(PasswordGenerationFlags.WithUpperCaseChars)]
        [InlineData(PasswordGenerationFlags.WithNumbers | PasswordGenerationFlags.WithLowerCaseChars)]
        [InlineData(PasswordGenerationFlags.Default | PasswordGenerationFlags.ReadableCharSet)]
        [InlineData(PasswordGenerationFlags.Default | PasswordGenerationFlags.EquallyDistributed)]
        [InlineData(PasswordGenerationFlags.WithNumbers | PasswordGenerationFlags.EquallyDistributed)]
        [InlineData(PasswordGenerationFlags.WithLowerCaseChars | PasswordGenerationFlags.EquallyDistributed)]
        [InlineData(PasswordGenerationFlags.WithUpperCaseChars | PasswordGenerationFlags.EquallyDistributed)]
        [InlineData(PasswordGenerationFlags.WithNumbers | PasswordGenerationFlags.WithLowerCaseChars | PasswordGenerationFlags.EquallyDistributed)]
        [InlineData(PasswordGenerationFlags.Default | PasswordGenerationFlags.ReadableCharSet | PasswordGenerationFlags.EquallyDistributed)]
        public void GeneratedPasswordsAreDifferent(PasswordGenerationFlags flags)
        {
            string lastPassword = null;
            for (int i = 0; i < 100; i++)
            {
                string password = Secrets.GeneratePassword(100, flags);
                Assert.Equal(100, password.Length);
                if (lastPassword == password)
                    throw new XunitException("Duplicate detected.");
                lastPassword = password;
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("test123")]
        [InlineData("this is a test string with spaces")]
        public void SecureCompareReturnsTrueForEqualStrings(string testString)
        {
            Assert.True(Secrets.SecureCompare(testString, new string(testString)));
        }

        [Theory]
        [InlineData("1", "2")]
        [InlineData("asdf", "jklö")]
        [InlineData("a", "aa")]
        [InlineData("this is", "a test")]
        public void SecureCompareReturnsFalseForDifferentStrings(string string1, string string2)
        {
            Assert.False(Secrets.SecureCompare(string1, string2));
        }
    }
}
